import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {tap} from 'rxjs/operators';
import {MyConfig} from '../../my-config';

@Injectable({
  providedIn: 'root'
})

export class CartService {
  private cartItems: any[] = [];
  private cartSubject = new BehaviorSubject<any[]>([]);

  private baseUrl = `${MyConfig.api_address}/api/cart`;

  constructor(
    private http: HttpClient,
  ) {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cartItems = JSON.parse(savedCart);
      this.cartSubject.next(this.cartItems);
    }
  }

  getCart(userId: string) {
    return this.http.get<any>(`${this.baseUrl}/view/${userId}`);
  }

  addToCart(userId: string, product: any) {
    const cartItem = {
      userId,
      productId: product.id,
      price: product.price,
      quantity: 1
    };

    this.http.post<any>(`${this.baseUrl}/add`, cartItem).subscribe(() => {
      this.getCart(userId);
    });
  }

  removeFromCart(cartItemId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/remove/${cartItemId}`);
  }

  updateCartItem(cartItemId: number, userId: string, quantity: number) {
    const updateRequest = { Quantity: quantity };
    return this.http.put<any>(`${this.baseUrl}/update/${cartItemId}`, updateRequest);
  }
}
