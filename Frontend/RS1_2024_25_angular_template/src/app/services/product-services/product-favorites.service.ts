import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {MyConfig} from '../../my-config';

@Injectable({
  providedIn: 'root'
})
export class FavoriteService {
  private favoriteProducts: any[] = [];
  private favoritesSubject = new BehaviorSubject<any[]>([]);

  favorites$ = this.favoritesSubject.asObservable();
  private baseUrl = `${MyConfig.api_address}/api/favorite`;


  constructor(
    private http: HttpClient,
  ) {
    const savedFavorites = localStorage.getItem('favorites');
    if (savedFavorites) {
      this.favoriteProducts = JSON.parse(savedFavorites);
      this.favoritesSubject.next(this.favoriteProducts);
    }
  }

  addFavorite(product: any) {
    const userId = localStorage.getItem('userId');

    if (!userId) {
      console.error('User ID not found.');
      return;
    }

    if (!this.favoriteProducts.some(p => p.id === product.id)) {
      const favoriteModel = {
        userId: userId,
        productId: product.id
      };

      this.http.post(this.baseUrl, favoriteModel).subscribe({
        next: () => {
          this.favoriteProducts.push(product);
          this.saveFavorites();
        },
        error: (err) => {
          console.error('Error saving favorite to backend:', err);
        }
      });
    }
  }

  removeFavorite(productId: number) {
    this.favoriteProducts = this.favoriteProducts.filter(p => p.id !== productId);
    this.saveFavorites();
  }

  private saveFavorites() {
    localStorage.setItem('favorites', JSON.stringify(this.favoriteProducts));
    this.favoritesSubject.next(this.favoriteProducts);
  }
}
