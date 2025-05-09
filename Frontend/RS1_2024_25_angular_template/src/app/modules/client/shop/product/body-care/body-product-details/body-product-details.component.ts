import {Component, OnInit} from '@angular/core';
import {Product, ProductReview, ProductService} from '../../../../../../services/product-services/product.service';
import {ActivatedRoute, Router} from '@angular/router';
import {MyConfig} from '../../../../../../my-config';
import {FavoriteService} from '../../../../../../services/product-services/product-favorites.service';
import {ReviewService} from '../../../../../../services/product-services/product-review.service';
import {MyAuthService} from '../../../../../../services/auth-services/my-auth.service';
import {HttpClient} from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import {CartService} from '../../../../../../services/product-services/product-cart.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MyDialogConfirmComponent} from '../../../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';

@Component({
  selector: 'app-body-product-details',
  standalone: false,
  templateUrl: './body-product-details.component.html',
  styleUrl: './body-product-details.component.css'
})
export class BodyProductDetailsComponent implements OnInit {
  product: any;
  productId!: number;
  reviews: ProductReview[] = [];
  averageRating: number = 0;
  newReview: ProductReview = {
    productId: 0,
    userId: localStorage.getItem('userId')!,
    productRating: 0,
    productComment: ''
  };
  selectedRating: number = 0;
  reviewText: string = "";
  updateReview: any = null;
  showUpdateForm: boolean = false;
  userId = localStorage.getItem('userId')!;
  private apiUrl = `${MyConfig.api_address}/api/ProductReview`;

  constructor(
    private productService: ProductService,
    private favoriteService: FavoriteService,
    private cartService: CartService,
    private productReviewService: ReviewService,
    private authService: MyAuthService,
    private route: ActivatedRoute,
    private http: HttpClient,
    private snack: MatSnackBar,
    private dialog: MatDialog
  ) {
  }

  ngOnInit() {
    const productId = Number(this.route.snapshot.paramMap.get('id'));
    if (productId) {
      this.productService.getProductById(productId).subscribe(
        (data) => {
          this.product = data;
        },
        (error) => {
          console.error('Greška pri dohvatanju proizvoda:', error);
        }
      );
    }

    this.route.params.subscribe(params => {
      this.productId = +params['id'];
      this.getReviews();
      this.getAverageRating();
    });
  }

  getImageUrl(imagePath: string): string {
    return `${MyConfig.api_address}${imagePath}`;
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

  getReviews(): void {
    this.productReviewService.getReviewsByProduct(this.productId).subscribe(response => {
      console.log('Recenzije:', response);
      if (response && response.$values && Array.isArray(response.$values)) {
        this.reviews = response.$values.map((review: any) => ({
          id: review.id,
          userId: review.userId,
          userFirstName: review.userFirstName,
          userLastName: review.userLastName,
          productId: review.productId,
          productName: review.productName,
          productRating: review.productRating,
          productComment: review.productComment,
          reviewDate: review.reviewDate,
          showMenu: false
        }));
      } else {
        console.error('Neispravan odgovor sa servera:', response);
      }
    });
  }

  getAverageRating(): void {
    this.productReviewService.getAverageRating(this.productId).subscribe(data => {
      this.averageRating = data.averageRating;
    });
  }

  addReview() {
    const userId = this.authService.getUserId();
    if (!userId) {
      console.error("User nije prijavljen!");
      return;
    }

    const reviewData = {
      userId: userId,
      productId: this.productId,
      productRating: this.selectedRating,
      productComment: this.reviewText
    };

    this.http.post(`${this.apiUrl}/add`, reviewData)
      .subscribe({
        next: (response) => {
          console.log("Recenzija dodana!", response);

          this.getReviews();

          this.selectedRating = 0;
          this.reviewText = "";
        },
        error: (error) => {
          console.error("Greška pri dodavanju recenzije:", error);
        }
      });
  }

  deleteReview(reviewId: number): void {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete confirmation',
        message: 'Are you sure you want to delete this review?',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.http.delete(`${this.apiUrl}/delete/${reviewId}`).subscribe({
          next: () => {
            console.log("Review deleted successfully!");
            this.getReviews();
          },
          error: (error) => {
            console.error("Error deleting review:", error);
          }
        });
      }
    });
  }


  toggleDropdown(review: any) {
    review.showMenu = !review.showMenu;
  }

  openUpdateReviewDialog(review: any): void {
    this.updateReview = { ...review };
    console.log(this.updateReview);
    this.showUpdateForm = true;
  }

  onCancelUpdate(): void {
    this.showUpdateForm = false;
  }

  onSaveUpdate() {
    if (!this.updateReview.id) {
      console.error("Nema ID-a recenzije!");
      return;
    }

    const updatedReviewData = {
      userId: this.updateReview.userId,
      productRating: this.updateReview.productRating,
      productComment: this.updateReview.productComment
    };

    this.http.put(`${this.apiUrl}/update/${this.updateReview.id}`, updatedReviewData)
      .subscribe({
        next: (response) => {
          console.log("Recenzija uspješno ažurirana!", response);
          this.showUpdateForm = false;
          this.getReviews();
        },
        error: (error) => {
          console.error("Greška pri ažuriranju recenzije", error);
        }
      });
  }

  addToCart() {
    this.cartService.addToCart(this.userId, this.product);
    this.showMessage("Product added to cart!");
  }
}
