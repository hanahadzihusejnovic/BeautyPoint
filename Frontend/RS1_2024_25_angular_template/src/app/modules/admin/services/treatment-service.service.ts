import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { MyConfig } from '../../../my-config';
import {Treatment, TreatmentResponse} from '../treatments/treatment/treatment-model';
import {catchError, map, tap} from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class TreatmentService {
  private apiUrl = `${MyConfig.api_address}/api/treatment`;
  private categoryApiUrl = `${MyConfig.api_address}/api/treatmentCategory`;

  constructor(private http: HttpClient) {}

  getTreatments(pageNumber: number, pageSize: number): Observable<TreatmentResponse> {
    return this.http.get<any>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`).pipe(
      map(response => {
        console.log('Raw API Response:', response);

        return {
          totalCount: response.totalCount,
          pageSize: response.pageSize,
          treatments: response.treatments?.$values ?? []
        };
      }),
      tap(parsedResponse => console.log('Parsed TreatmentResponse:', parsedResponse))
    );
  }

  createTreatment(treatment: any): Observable<any> {
    return this.http.post(this.apiUrl, treatment);
  }

  updateTreatment(id: number, treatment: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, treatment);
  }

  deleteTreatment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getTreatmentCategories(): Observable<any[]> {
    return this.http.get<any>(this.categoryApiUrl).pipe(
      map((response) => response.$values || []),
      catchError((error) => {
        console.error('Error fetching treatment categories:', error);
        return [];
      })
    );
  }
}
