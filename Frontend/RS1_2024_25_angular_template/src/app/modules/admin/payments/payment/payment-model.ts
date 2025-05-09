import {Product} from '../../products/product/product-model';

export interface Payment {
  id: number;
  orderId: number;
  paymentDate: string;
  amount: number;
  paymentMethod: string;
  paymentStatus: string;
  transactionId: number;
}

export interface PaymentResponse {
  totalCount: number;
  pageSize: number;
  payments: Payment[];
}
