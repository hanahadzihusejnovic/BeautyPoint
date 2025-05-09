import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from '../../../../../services/product-services/product.service';
import { FavoriteService } from '../../../../../services/product-services/product-favorites.service';
import {User} from '../../../../admin/users/user/user-model';
import {Router} from '@angular/router';
import {CartService} from '../../../../../services/product-services/product-cart.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MyConfig} from '../../../../../my-config';

@Component({
  selector: 'app-hair-care',
  standalone: false,
  templateUrl: './hair-care.component.html',
  styleUrls: ['./hair-care.component.css'],
})
export class HairCareComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  productCategoryId: number = 1;
  selectedVolume: number | undefined = undefined;
  searchText: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  sortedColumn: string = '';
  sortBy: keyof Product = 'productName';
  showSortDropdown: boolean = false;
  userId = localStorage.getItem('userId')!;

  currentPage: number = 1;
  pageSize: number = 12;
  totalItems: number = 0;

  constructor(
    private productService: ProductService,
    private favoriteService: FavoriteService,
    private cartService: CartService,
    private router: Router,
    private snack: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadHairCareProducts();
  }

  loadHairCareProducts(): void {
    this.productService.getProductsByCategory(this.productCategoryId, this.currentPage, this.pageSize).subscribe(
      (response) => {
        console.log('Loaded Hair Care products:', response);

        this.totalItems = response.totalCount;
        this.products = response.products.$values.map((product) => ({
          ...product,
          imageUrl: `${MyConfig.api_address}${product.imagePath}`,
        }));

        this.filteredProducts = [...this.products];
      },
      (error) => {
        console.error('Error loading Hair Care products:', error);
      }
    );
  }

  onVolumeChange(event: Event): void {
    const volume = (event.target as HTMLSelectElement).value;
    this.selectedVolume = volume ? parseInt(volume, 10) : undefined;

    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    const lowerCaseSearchText = this.searchText.toLowerCase();

    this.filteredProducts = this.products.filter((product) => {
      const matchesSearchText = product.productName.toLowerCase().includes(lowerCaseSearchText);
      const matchesVolume = this.selectedVolume ? product.volume === this.selectedVolume : true;
      return matchesSearchText && matchesVolume;
    });

    this.sortProducts(this.sortBy);
  }

  showMessage(message: string) {
    this.snack.open(message, 'OK', {
      duration: 4000,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }

  onAddToFavorites(product: any) {
    this.favoriteService.addFavorite(product);
    this.showMessage("Product added to favorites!");
  }

  onAddToCart(product: any) {
    this.cartService.addToCart(this.userId, product);
    this.showMessage("Product added to cart!");
  }

  toggleSortDropdown(): void {
    this.showSortDropdown = !this.showSortDropdown;
  }

  onSortChange(): void {
    this.sortProducts(this.sortBy);
  }

  sortProducts(column: keyof Product): void {
    const direction = this.sortDirection === 'asc' ? 1 : -1;

    this.filteredProducts.sort((a, b) => {
      const valueA = a[column] as string | number;
      const valueB = b[column] as string | number;

      if (valueA < valueB) return -1 * direction;
      if (valueA > valueB) return 1 * direction;
      return 0;
    });
  }

  onDragStart(event: DragEvent, product: Product){
    event.dataTransfer?.setData("application/json", JSON.stringify(product));
    event.dataTransfer!.effectAllowed = "copy";
  }


  onDrop(event: DragEvent) {
    event.preventDefault();

    const data = event.dataTransfer?.getData("application/json");
    if (data) {
      const product: any = JSON.parse(data);
      this.favoriteService.addFavorite(product);
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.dataTransfer!.dropEffect = "copy";
  }

  goToDetails(id: number){
    this.router.navigate(['/hair-product-details', id]);
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalItems) {
      this.currentPage++;
      this.loadHairCareProducts();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadHairCareProducts();
    }
  }
}
