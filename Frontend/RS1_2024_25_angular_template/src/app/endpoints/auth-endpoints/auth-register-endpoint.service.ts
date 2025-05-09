import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterDto } from '../../services/auth-services/dto/register-dto';
import {MyConfig} from '../../my-config';

@Injectable({ providedIn: 'root' })
export class AuthRegisterEndpointService {
  private apiUrl = `${MyConfig.api_address}/api/user`;

  constructor(private httpClient: HttpClient) {}

  register(userData: RegisterDto): Observable<any> {
    console.log('Sending data to API:', userData);
    return this.httpClient.post(this.apiUrl, userData);
  }
}
