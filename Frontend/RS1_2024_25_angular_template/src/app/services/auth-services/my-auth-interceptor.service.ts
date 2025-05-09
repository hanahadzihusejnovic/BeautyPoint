import { Injectable } from "@angular/core";
import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { MyAuthService } from "./my-auth.service";
import { MyPageProgressbarService } from "../../modules/shared/progressbars/my-page-progressbar.service";
import { catchError, finalize, switchMap, throwError, of } from "rxjs";

@Injectable()
export class MyAuthInterceptor implements HttpInterceptor {
  constructor(
    private auth: MyAuthService,
    private progressBarService: MyPageProgressbarService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    let authToken = this.auth.getLoginToken()?.accessToken ?? "";

    if (!authToken) {
      console.log("No access token available, skipping request.");
      return next.handle(req);
    }

    this.progressBarService.show();

    const authReq = req.clone({
      headers: req.headers.set("Authorization", `Bearer ${authToken}`),
    });

    return next.handle(authReq).pipe(
      catchError((error) => {
        if (error.status === 401 || error.status === 403) {
          console.log("Unauthorized or forbidden - triggering refresh");
          return this.auth.refreshToken().pipe(
            switchMap((newTokens) => {
              if (newTokens && newTokens.accessToken) {
                console.log("Tokens updated successfully.");
                const updatedToken = newTokens.accessToken;
                const updatedAuthReq = req.clone({
                  headers: req.headers.set("Authorization", `Bearer ${updatedToken}`),
                });
                return next.handle(updatedAuthReq);
              }
              console.error("Failed to refresh token.");
              this.auth.logout();
              return throwError(() => new Error("Unauthorized"));
            })
          );
        }
        return throwError(() => error);
      }),
      finalize(() => {
        this.progressBarService.hide();
      })
    );
  }
}
