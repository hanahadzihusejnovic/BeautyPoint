import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable, of, throwError} from 'rxjs';
import { ProductResponse } from '../products/product/product-model';
import { MyConfig } from '../../../my-config';
import {catchError, map, tap} from 'rxjs/operators';
import {Product} from '../products/product/product-model';
import {ProductCategory} from '../products/product/product-category-model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${MyConfig.api_address}/api/product`;

  constructor(private http: HttpClient) {}

  getProducts(pageNumber: number = 1, pageSize: number = 10): Observable<ProductResponse> {
    return this.http.get<any>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`).pipe(
      map(response => {
        console.log('Raw API Response:', response);

        return {
          totalCount: response.totalCount,
          pageSize: response.pageSize,
          products: response.products?.$values ?? []
        };
      }),
      tap(parsedResponse => console.log('Parsed ProductResponse:', parsedResponse))
    );
  }


  createProduct(formData: FormData): Observable<any> {
    return this.http.post(this.apiUrl, formData);
  }

  updateProduct(id: number, formData: FormData): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, formData).pipe(
      catchError(error => {
        console.error("Error updating product:", error);
        return throwError(error);
      })
    );
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCategories(): Observable<ProductCategory[]> {
    return this.http.get<any>(`${MyConfig.api_address}/api/productCategory`).pipe(
      map(response => response.$values as ProductCategory[])
    );
  }


}
