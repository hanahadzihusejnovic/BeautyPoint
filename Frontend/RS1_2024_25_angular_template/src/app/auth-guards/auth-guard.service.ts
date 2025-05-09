import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { MyAuthService } from '../services/auth-services/my-auth.service';

export class AuthGuardData {
  isAdmin?: boolean;
  isEmployee?: boolean;
  isClient?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: MyAuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const guardData = route.data as AuthGuardData;

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    if (guardData.isAdmin && !this.authService.isAdmin()) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    if (guardData.isEmployee && !this.authService.isEmployee()) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    if (guardData.isClient && !this.authService.isClient()) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}
