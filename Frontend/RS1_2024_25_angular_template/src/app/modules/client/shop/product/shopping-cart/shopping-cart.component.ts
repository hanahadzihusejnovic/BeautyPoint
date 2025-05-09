import { Component, OnInit } from '@angular/core';
import { CartService } from '../../../../../services/product-services/product-cart.service';
import { MyConfig } from '../../../../../my-config';
import { HttpClient } from '@angular/common/http';
import { CheckoutModel } from '../checkout.model';
import { Router } from '@angular/router';
import { SavedItemsService } from '../../../../../services/product-services/product-saved-items.service';
import { MatDialog } from '@angular/material/dialog';
import {MyDialogSimpleComponent} from '../../../../shared/dialogs/my-dialog-simple/my-dialog-simple.component';

import {
  Stripe,
  loadStripe,
  StripeElements,
  StripeElementsOptions,
  StripeElementsOptionsClientSecret
} from '@stripe/stripe-js';
import { StripePaymentService } from '../../../../../services/stripe-payment.service';
import { environment } from '../../../../../../environment';
import {catchError} from 'rxjs/operators';
import {throwError} from 'rxjs';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-shopping-cart',
  standalone: false,
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit {
  cartItems: any[] = [];
  userId = localStorage.getItem('userId')!;
  checkoutModel: CheckoutModel = { userId: '', address: '', paymentMethod: '' };
  totalPrice: number = 0;
  showCheckoutForm: boolean = false;

  stripe!: Stripe | null;
  elements!: StripeElements;
  cardElement: any;
  elementsOptions!: StripeElementsOptionsClientSecret;

  constructor(
    private cartService: CartService,
    private savedItemsService: SavedItemsService,
    private http: HttpClient,
    private router: Router,
    private stripePaymentService: StripePaymentService,
    private dialog: MatDialog,
    private snack: MatSnackBar
  ) {}

  async ngOnInit() {
    this.cartService.getCart(this.userId).subscribe(cart => {
      console.log('Cart response:', cart);
      this.cartItems = cart.cartItems?.$values || [];
      this.calculateTotalPrice();

      if (this.totalPrice <= 0) {
        console.warn('Cart is empty. No need to create PaymentIntent.');
        return;
      }
      this.initializeStripe();
    });
  }

  async initializeStripe() {
    this.stripe = await loadStripe(environment.stripePublishableKey, {
      locale: 'en'
    });

    if (!this.stripe) {
      console.error('Stripe not loaded.');
      return;
    }

    console.log('Stripe successfully loaded.');

    this.http.post<any>(`${environment.api_address}/api/payment/create-payment-intent`, {
      orderId: this.cartItems[0]?.orderId,
      amount: this.totalPrice
    }).subscribe(async (response) => {
      if (response?.clientSecret) {
        console.log('ClientSecret:', response.clientSecret);

        this.elementsOptions = {
          clientSecret: response.clientSecret,
          appearance: { theme: 'stripe' }
        };

        const style = {
          base: {
            color: "#5a2424",
            fontSize: "1em",
            fontWeight: "bold",
            fontFamily: "Arial, sans-serif",
            backgroundColor: "#f5c6c6",
            padding: "10px",
            border: "1px solid #d49e9e",
            borderRadius: "8px",
            "::placeholder": {
              color: "#5a2424",
              fontSize: "1em",
            },
          },
          invalid: {
            color: "#ff4d4d",
          },
        };


        this.elements = this.stripe!.elements(this.elementsOptions);
        this.cardElement = this.elements.create('card', { style });
        this.cardElement.mount('#card-element');
      } else {
        console.error('No client secret in response.');
      }
    }, error => {
      console.error('Error creating PaymentIntent:', error);
    });

  }

  calculateTotalPrice() {
    this.totalPrice = this.cartItems.reduce((sum, item) => sum + item.price, 0);
  }

  removeFromCart(cartItemId: number) {
    this.cartService.removeFromCart(cartItemId).subscribe(() => {
      this.cartItems = this.cartItems.filter(item => item.id !== cartItemId);
      this.calculateTotalPrice();
    }, error => {
      console.error('Error removing from cart:', error);
    });

    this.showMessage("Product removed from cart!");
  }

  getImageUrl(imagePath: string): string {
    return `${MyConfig.api_address}${imagePath}`;
  }

  updateQuantity(item: any, change: number) {
    const newQuantity = item.quantity + change;
    if (newQuantity > 0) {
      this.cartService.updateCartItem(item.id, this.userId, newQuantity)
        .subscribe(updatedItem => {
          item.quantity = updatedItem.quantity;
          item.price = updatedItem.price;
          this.calculateTotalPrice();
        });
    }
  }

  async onCheckout() {
    if (!this.checkoutModel.address || !this.checkoutModel.paymentMethod) {
      return;
    }

    this.checkoutModel.userId = this.userId;

    if (!this.checkoutModel.userId) {
      console.error('UserId not found!');
      this.openDialog('Error', 'An error occurred. Please log in again.');
      return;
    }

    if (this.checkoutModel.paymentMethod === 'Card' && (!this.stripe || !this.cardElement)) {
      console.error('Stripe not initialized.');
      this.openDialog('Payment Error', 'There was an issue with the payment. Please try again.');
      return;
    }

    try {
      const order = await this.createOrder(this.checkoutModel);
      console.log('Server Response:', order);

      if (!order || !order.orderId) {
        console.error('Error creating order. Order ID not found.');
        this.openDialog('Order Error', 'Error occurred while creating the order.');
        return;
      }

      console.log('Order created:', order);

      console.log('Client Secret before confirmCardPayment:', this.elementsOptions?.clientSecret);

      if (!this.elementsOptions?.clientSecret) {
        console.error('Client Secret not found!');
        this.openDialog('Payment Error', 'An error occurred. No Client Secret found.');
        return;
      }

      if (this.checkoutModel.paymentMethod === 'Card') {
        const { error, paymentIntent } = await this.stripe!.confirmCardPayment(
          this.elementsOptions.clientSecret!,
          {
            payment_method: {
              card: this.cardElement,
              billing_details: {
                name: 'Test User',
                email: 'test@example.com'
              }
            }
          }
        );

        if (error) {
          console.error('Error while paying:', error.message);
          this.openDialog('Payment Failed', 'Payment was not successful. Please try again.');
          return;
        }

        if (paymentIntent?.status === 'succeeded') {
          const paymentData = {
            orderId: order.orderId,
            paymentIntentId: paymentIntent.id
          };

          await this.savePayment(paymentData);
          this.openDialog('Success', 'Payment successful and saved! Thank you for your purchase.');
          this.router.navigate(['/shopping-cart']);
        }
      } else {
        this.openDialog('Order Created', 'Your order has been successfully created. Payment will be made upon delivery.');
        this.router.navigate(['/shopping-cart']);
      }
    } catch (error) {
      console.error('Error:', error);
      this.openDialog('Processing Error', 'An error occurred while processing the order or payment.');
    }
  }

  openDialog(title: string, message: string): void {
    this.dialog.open(MyDialogSimpleComponent, {
      data: { title, message }
    });
  }

  createOrder(checkoutModel: CheckoutModel) {
    return this.http.post<any>(`${environment.api_address}/api/checkout`, checkoutModel).toPromise()
      .then(response => {
        console.log('Server Response:', response);
        return response;
      })
      .catch(error => {
        console.error('Error while creating an order:', error);
        throw error;
      });
  }

  savePayment(paymentData: any) {
    return this.http.post<any>(`${environment.api_address}/api/payment/confirm-payment`, paymentData).toPromise();
  }

  onPaymentMethodChange() {
    console.log('Chosen payment method:', this.checkoutModel.paymentMethod);

    if (this.checkoutModel.paymentMethod === 'Card') {
      this.initializeStripeElements();
    }
  }

  async initializeStripeElements() {
    if (!this.stripe) {
      this.stripe = await loadStripe(environment.stripePublishableKey);
    }

    if (!this.stripe) {
      console.error('Stripe nije uspješno inicijalizovan.');
      return;
    }

    const elements = this.stripe.elements();
    this.cardElement = elements.create('card');

    setTimeout(() => {
      const cardElementContainer = document.getElementById('card-element');
      if (!cardElementContainer) {
        console.error('Element #card-element nije pronađen u DOM-u.');
        return;
      }

      this.cardElement.mount('#card-element');
      console.log('Stripe card element uspješno mountovan.');
    }, 100);
  }

  showMessage(message: string) {
    this.snack.open(message, 'OK', {
      duration: 4000,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }

  saveForLater(product: any) {
    const quantity = product.quantity ?? 1;
    this.savedItemsService.saveForLater(this.userId, product.id, quantity).subscribe(response => {
      this.savedItemsService.removeFromCart(this.userId, product.id).subscribe(() => {
        this.cartItems = this.cartItems.filter(item => item.product.id !== product.id);
        this.calculateTotalPrice();
      });
    }, error => {
      console.error('Error saving product for later:', error);
    });

    this.showMessage("Product saved for later!");
  }
}
