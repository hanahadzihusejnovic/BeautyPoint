import { Component, OnInit } from '@angular/core';
import {ProductService} from '../../services/products-service.service';
import { Product} from './product-model';
import {MyConfig} from '../../../../my-config';
import {MatDialog} from '@angular/material/dialog';
import {CreateProductComponent} from '../create-product/create-proudct.component';
import {UpdateProductComponent} from '../update-product/update-product/update-product.component';
import {MyDialogConfirmComponent} from '../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import { Location } from '@angular/common';

@Component({
  selector: 'app-product',
  standalone: false,

  templateUrl: './product.component.html',
  styleUrl: './product.component.css'
})
export class ProductComponent implements OnInit {
  products: Product[] = [];
  searchQuery: string = '';
  activeProductId: number | null = null;

  currentPage: number = 1;
  pageSize: number = 10;
  totalProducts: number = 0;

  constructor(private productService: ProductService, private dialog: MatDialog, private location: Location) {}

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getProducts(this.currentPage, this.pageSize).subscribe((response) => {
      console.log('Data received:', response);

      if (response) {
        this.products = response.products;
        this.totalProducts = response.totalCount;
        this.pageSize = response.pageSize;
      } else {
        console.error('Unexpected response format:', response);
      }
    });
  }

  filterProducts(): Product[] {
    return this.products.filter((product) =>
      product.productName.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  toggleActions(productId: number): void {
    this.activeProductId = this.activeProductId === productId ? null : productId;
  }

  openUpdateProductModal(product: Product): void {
    console.log('Product to update:', product);

    const dialogRef = this.dialog.open(UpdateProductComponent, {
      width: '500px',
      disableClose: true,
      data: { product }
    });

    dialogRef.afterClosed().subscribe((productUpdated) => {
      if (productUpdated) {
        this.loadProducts();
      }
    });
  }

  deleteProduct(id: number): void {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete confirmation',
        message: 'Are you sure you want to delete this product?',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.productService.deleteProduct(id).subscribe(() => {
          this.products = this.products.filter(p => p.id !== id);
        });
      }
    });
  }

  getImageUrl(imagePath: string): string {
    return `${MyConfig.api_address}${imagePath}`;
  }

  openCreateProductModal(): void {
    const dialogRef = this.dialog.open(CreateProductComponent, {
      width: '500px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((productCreated) => {
      if (productCreated) {
        this.loadProducts();
      }
    });
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalProducts) {
      this.currentPage++;
      this.loadProducts();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadProducts();
    }
  }

  goBack(): void {
    this.location.back();
  }

}
