import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {MyConfig} from '../../../my-config';
import {Payment, PaymentResponse} from '../payments/payment/payment-model';
import {map, tap} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PaymentsService {

  private apiUrl = `${MyConfig.api_address}/api/payment`;

  constructor(private http: HttpClient) {}

  getPayments(pageNumber: number, pageSize: number): Observable<PaymentResponse> {
    return this.http.get<any>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`).pipe(
      map(response => {
        console.log('Raw API Response:', response);
        return {
          totalCount: response.totalCount,
          pageSize: response.pageSize,
          payments: response.payments?.$values ?? []
        } as PaymentResponse;
      }),
      tap(parsedResponse => console.log('Parsed PaymentResponse:', parsedResponse))
    );
  }

  createPayment(payment: any): Observable<any> {
    return this.http.post(this.apiUrl, payment);
  }

  updatePayment(id: number, payment: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, payment);
  }

  deletePayment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
