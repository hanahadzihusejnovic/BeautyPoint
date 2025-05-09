import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../../../my-config';
import { UserResponse} from '../users/user/user-model';
import {map, tap} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private apiUrl = `${MyConfig.api_address}/api/user`;

  constructor(private http: HttpClient) {}

  getUsers(pageNumber: number, pageSize: number): Observable<UserResponse> {
    return this.http.get<any>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`).pipe(
      map(response => {
        console.log('Raw API Response:', response);

        return {
          totalCount: response.totalCount,
          pageSize: response.pageSize,
          users: response.users?.$values ?? []
        } as UserResponse;
      }),
      tap(parsedResponse => console.log('Parsed UserResponse:', parsedResponse))
    );
  }

  createUser(user: any): Observable<any> {
    return this.http.post(this.apiUrl, user);
  }

  updateUser(id: string, user: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, user, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
