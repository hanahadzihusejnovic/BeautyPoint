import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {MyConfig} from '../../my-config';

export interface ProductReviewCreateDto {
  userId: string;
  productId: number;
  productRating: number;
  productComment: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private apiUrl = `${MyConfig.api_address}/api/ProductReview`;
  constructor(private http: HttpClient) {}

  getReviewsByProduct(productId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/product/${productId}`);
  }

  getAverageRating(productId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/product/${productId}/average-rating`);
  }

  addReview(review: ProductReviewCreateDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/add`, review);
  }

  updateReview(reviewId: number, review: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${reviewId}`, review);
  }

  deleteReview(reviewId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${reviewId}`);
  }
}
