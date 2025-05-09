import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../../../my-config';
import { User } from '../reservations/reservation/user-model';
import { Service } from '../reservations/reservation/service-model';
import {Reservation, ReservationResponse} from '../reservations/reservation/reservation-model';
import {catchError, map, tap} from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ReservationService {
  private apiUrl = `${MyConfig.api_address}/api/reservation`;
  private usersUrl = `${MyConfig.api_address}/api/user/all-dropdown`;
  private servicesUrl = `${MyConfig.api_address}/api/treatment/all-dropdown`;

  constructor(private http: HttpClient) {}

  getReservations(pageNumber: number, pageSize: number): Observable<ReservationResponse> {
    return this.http.get<any>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`).pipe(
      map(response => {
        console.log('Raw API Response:', response);

        return {
          totalCount: response.totalCount,
          pageSize: response.pageSize,
          reservations: response.reservations?.$values ?? []
        };
      }),
      tap(parsedResponse => console.log('Parsed ReservationResponse:', parsedResponse))
    );
  }


  createReservation(reservation: any): Observable<any> {
    return this.http.post(this.apiUrl, reservation);
  }

  updateReservation(id: number, reservation: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, reservation);
  }

  deleteReservation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<any>(this.usersUrl).pipe(
      map((response) => response.$values || []),
      catchError((error) => {
        console.error('Error fetching users:', error);
        return [];
      })
    );
  }

  getServices(): Observable<Service[]> {
    return this.http.get<any>(this.servicesUrl).pipe(
      map((response) => response.$values || []),
      catchError((error) => {
        console.error('Error fetching services:', error);
        return [];
      })
    );
  }
}
