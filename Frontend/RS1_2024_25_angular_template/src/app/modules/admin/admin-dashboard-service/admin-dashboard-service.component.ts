import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../../../my-config';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';

@Injectable({
  providedIn: 'root',
})
export class AdminDashboardService {
  private apiUrl = `${MyConfig.api_address}/api/admin/dashboard-stats`;

  constructor(private http: HttpClient, private authService: MyAuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const loginToken = this.authService.getLoginToken();
    if (loginToken && loginToken.accessToken) {
      return new HttpHeaders().set('Authorization', `Bearer ${loginToken.accessToken}`);
    }
    return new HttpHeaders();
  }

  getDashboardStats(): Observable<DashboardStats> {
    const headers = this.getAuthHeaders();
    return this.http.get<DashboardStats>(this.apiUrl, { headers });
  }
}

export interface DashboardStats {
  totalProducts: number;
  totalReservations: number;
  totalClients: number;
  totalRevenue: number;
  totalTreatments: number;
}
