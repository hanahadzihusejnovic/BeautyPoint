import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import { MyConfig} from '../../my-config';

@Injectable({
  providedIn: 'root'
})

export class SavedItemsService {
  private savedItems: any[] = [];
  private savedSubject = new BehaviorSubject<any[]>([]);

  private baseUrl = `${MyConfig.api_address}/api/saved-items`;

  constructor(private http: HttpClient) {
    const savedItems = localStorage.getItem('savedItems');
    if(savedItems) {
      this.savedItems = JSON.parse(savedItems);
      this.savedSubject.next(this.savedItems);
    }
  }

  saveForLater(userId: string, productId: number, quantity: number): Observable<any> {
    if (quantity <= 0) {
      quantity = 1;
  }
    const request = { userId, productId, quantity };
    console.log('Sending request to save-for-later:', request);
    return this.http.post(`${this.baseUrl}/save-for-later`, request);
  }

  getSavedItems(userId: string) {
    return this.http.get<any[]>(`${this.baseUrl}/get-saved-items/${userId}`);
  }

  removeFromSaved(userId: string, productId: number) {
    return this.http.delete(`${this.baseUrl}/remove/${userId}/${productId}`);
  }

  removeFromCart(userId: string, productId: number) {
    return this.http.delete(`${this.baseUrl}/removeCart/${userId}/${productId}`);
  }

  moveToCart(userId: string, productId: number, quantity: number): Observable<any> {
    if (quantity <= 0) {
      quantity = 1;
    }
    const request = { userId, productId, quantity };
    return this.http.post(`${this.baseUrl}/move-to-cart`, request);
  }
}

