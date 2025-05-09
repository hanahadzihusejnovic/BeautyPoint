import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, of, throwError } from "rxjs";
import { catchError, switchMap } from "rxjs/operators";
import { MyAuthInfo } from "./dto/my-auth-info";
import { LoginTokenDto } from "./dto/login-token-dto";
import { MyConfig } from '../../my-config';

@Injectable({ providedIn: "root" })
export class MyAuthService {
  private readonly apiUrl = MyConfig.api_address;
  constructor(private httpClient: HttpClient) {}

  getMyAuthInfo(): MyAuthInfo | null {
    return this.getLoginToken()?.myAuthInfo ?? null;
  }

  isLoggedIn(): boolean {
    return this.getMyAuthInfo()?.isLoggedIn ?? false;
  }

  getUserRole(): string | null {
    return this.getMyAuthInfo()?.role ?? null;
  }

  isAdmin(): boolean {
    return this.getUserRole() === "Admin";
  }

  isClient(): boolean {
    return this.getUserRole() === "Client";
  }

  isEmployee(): boolean {
    return this.getUserRole() === "Employee";
  }

  setLoggedInUser(tokens: LoginTokenDto | null): void {
    if (tokens == null) {
      window.localStorage.removeItem("my-auth-token");
      window.localStorage.removeItem("userId");
    } else {
      window.localStorage.setItem("my-auth-token", JSON.stringify(tokens));

      if (tokens.myAuthInfo?.userId) {
        window.localStorage.setItem("userId", tokens.myAuthInfo.userId);
      }
    }
  }

  getUserId(): string | null {
    return localStorage.getItem("userId");
  }


  getLoginToken(): LoginTokenDto | null {
    const tokenString = window.localStorage.getItem("my-auth-token") ?? "";
    console.log("Token string from localStorage:", tokenString);
    try {
      const parsedToken = JSON.parse(tokenString) as LoginTokenDto;
      console.log("Parsed token object:", parsedToken);
      return parsedToken;
    } catch (e) {
      console.error("Error parsing token from localStorage:", e);
      return null;
    }
  }


  logout(): void {
    this.setLoggedInUser(null);
  }

  isTokenExpired(token: string): boolean {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const decoded = JSON.parse(atob(base64));

      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp < currentTime;
    } catch (e) {
      console.error("Error decoding token:", e);
      return true;
    }
  }

  refreshToken(): Observable<LoginTokenDto> {
    const refreshToken = this.getLoginToken()?.refreshToken;
    const accessToken = this.getLoginToken()?.accessToken;

    if (accessToken && !this.isTokenExpired(accessToken)) {
      console.log("Access token is still valid, no refresh required.");
      return of(this.getLoginToken()!);
    }

    if (!refreshToken) {
      console.error("Refresh token is missing. Check if user is logged in and tokens are stored correctly.");
      return throwError(() => new Error("Refresh token is missing"));
    }

    console.log("Attempting to refresh token...");
    console.log("Current refresh token:", refreshToken);

    return this.httpClient.post<any>(`${this.apiUrl}/api/auth/refresh-token`, { refreshToken }).pipe(
      switchMap((response: any) => {
        console.log("Response from refresh token endpoint:", response);
        if (response.accessToken && response.refreshToken) {
          const updatedTokens: LoginTokenDto = {
            accessToken: response.accessToken,
            refreshToken: response.refreshToken,
            myAuthInfo: this.getMyAuthInfo(),
          };
          this.setLoggedInUser(updatedTokens);
          console.log("Tokens updated successfully.");
          return of(updatedTokens);
        } else {
          console.error("Invalid response during token refresh");
          return throwError(() => new Error("Invalid response during token refresh"));
        }
      }),
      catchError((err) => {
        console.error("Error refreshing token:", err);
        this.logout();
        return throwError(() => err);
      })
    );
  }


}
