import { Component, OnInit } from '@angular/core';
import { SavedItemsService} from '../../../../../services/product-services/product-saved-items.service';
import {MyConfig} from '../../../../../my-config';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-save-for-later',
  standalone: false,
  templateUrl: './save-for-later.component.html',
  styleUrls: ['./save-for-later.component.css']
})
export class SaveForLaterComponent implements OnInit {
  savedItems: any[] = [];
  userId = localStorage.getItem('userId')!;

  constructor(
    private savedItemsService: SavedItemsService,
    private snack: MatSnackBar
    ) {}

  ngOnInit(): void {
    this.loadSavedItems();
  }

  getImageUrl(imagePath: string): string {
    return `${MyConfig.api_address}${imagePath}`;
  }

  loadSavedItems() {
    this.savedItemsService.getSavedItems(this.userId).subscribe((savedItem: any) => {
      console.log(savedItem);
      this.savedItems = savedItem.$values || [];
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

  moveToCart(productId: number) {
    const item = this.savedItems.find(savedItem => savedItem.product.id === productId);
    if (item) {
      const quantity = item.quantity;
      this.savedItemsService.moveToCart(this.userId, productId, quantity).subscribe(
        (response: any) => {
          console.log('Response from moveToCart:', response);
          this.removeFromSaved(productId);
          this.loadSavedItems();
        },
        (error) => {
          console.error('Error moving item to cart:', error);
        }
      );
    }
  }

  removeFromSaved(productId: number) {
    this.savedItemsService.removeFromSaved(this.userId, productId).subscribe(response => {
      this.savedItems = this.savedItems.filter(item => item.product.id !== productId);
    });
    this.showMessage("Product removed from saved!");
  }
}
