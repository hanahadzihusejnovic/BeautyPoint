import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {MyConfig} from '../my-config';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = `${MyConfig.api_address}/api/user`;
  constructor(private http: HttpClient) {}

  checkUsernameExists(username: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/check-username?username=${username}`);
  }
}
