import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {map, tap} from 'rxjs/operators';
import {ProductResponse} from '../../modules/admin/products/product/product-model';
import {MyConfig} from '../../my-config';

interface ProductCategory {
  id: number;
  name: string;
}

export interface Product {
  id: number;
  productName: string;
  productPrice: number;
  productDescription: string;
  productCategoryId: number;
  volume: number;
  imagePath: string;
  imageUrl?: string;
  productReviews: ProductReview[];
}

export interface ProductReview {
  id?: number;
  userId: string;
  userFirstName?: string;
  userLastName?: string;
  productId: number;
  productName?: string;
  productRating: number;
  productComment: string;
  reviewDate?: Date;
  showMenu?: boolean;
}

interface PagedResponse<T> {
  totalCount: number;
  pageSize: number;
  products: {
    $values: T[];
  };
}

@Injectable({
  providedIn: 'root',
})

export class ProductService {
  private baseUrl = `${MyConfig.api_address}/api/product`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('authToken');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  getAllProducts(): Observable<Product[]> {
    return this.http.get<PagedResponse<Product>>(this.baseUrl, { headers: this.getHeaders() }).pipe(
      map((response) => response.products.$values)
    );
  }

  getProductById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`);
  }

  getProductsByCategory(productCategoryId: number, pageNumber: number, pageSize: number): Observable<PagedResponse<Product>> {
    return this.http.get<PagedResponse<Product>>(`${this.baseUrl}?productCategoryId=${productCategoryId}&pageNumber=${pageNumber}&pageSize=${pageSize}`, {
      headers: this.getHeaders()
    }).pipe(
      tap(response => console.log('API Response: ',response))
    );
  }
}
