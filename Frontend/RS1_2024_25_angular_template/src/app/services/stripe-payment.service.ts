import { Injectable } from '@angular/core';
import { Stripe, StripeElements, StripeElementsOptions, loadStripe } from '@stripe/stripe-js';
import { HttpClient } from '@angular/common/http';
import { environment} from '../../environment';


@Injectable({
  providedIn: 'root'
})
export class StripePaymentService {
  stripePromise = loadStripe(environment.stripePublishableKey);

  constructor(private http: HttpClient) {}

  createPaymentIntent(data: any) {
    return this.http.post(`${environment.api_address}/api/payment/create-payment-intent`, data);
  }

  checkout(data: any) {
    return this.http.post(`${environment.api_address}/api/checkout`, data);
  }

}
