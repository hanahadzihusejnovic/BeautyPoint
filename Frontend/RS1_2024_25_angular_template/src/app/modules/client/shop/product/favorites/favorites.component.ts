import { Component, OnInit } from '@angular/core';
import { FavoriteService } from '../../../../../services/product-services/product-favorites.service';
import {MyConfig} from '../../../../../my-config';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-favorites',
  standalone: false,
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.css'
})
export class FavoritesComponent implements OnInit {
  favoriteProducts: any[] = [];

  constructor(
    private favoriteService: FavoriteService,
    private snack: MatSnackBar,
  ) {}

  ngOnInit() {
    this.favoriteService.favorites$.subscribe(products => {
      this.favoriteProducts = products;
    });
  }

  showMessage(message: string) {
    this.snack.open(message, 'OK', {
      duration: 4000,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }

  removeFromFavorites(productId: number) {
    this.favoriteService.removeFavorite(productId);
    this.showMessage("Product removed from favorites!");
  }

  getImageUrl(imagePath: string): string {
    return `${MyConfig.api_address}${imagePath}`;
  }
}
