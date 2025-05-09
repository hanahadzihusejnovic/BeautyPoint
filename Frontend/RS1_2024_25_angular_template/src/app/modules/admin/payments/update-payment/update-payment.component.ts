import {ChangeDetectorRef, Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Treatment} from '../../treatments/treatment/treatment-model';
import {Payment} from '../payment/payment-model';
import {TreatmentService} from '../../services/treatment-service.service';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {PaymentsService} from '../../services/payments-service.service';

@Component({
  selector: 'app-update-payment',
  standalone: false,

  templateUrl: './update-payment.component.html',
  styleUrl: './update-payment.component.css'
})
export class UpdatePaymentComponent implements OnInit {
  paymentForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  currentPayment: Payment;

  paymentMethods = ['Card', 'Cash on delivery'];
  paymentStatuses = ['Pending', 'Completed', 'Failed'];


  constructor(
    private fb: FormBuilder,
    private paymentService: PaymentsService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<UpdatePaymentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { payment: Payment }
  ) {
    this.currentPayment = data.payment;
  }

  ngOnInit(): void {
    console.log('Current Treatment:', this.currentPayment);

    this.paymentForm = this.fb.group({
      orderId: [this.currentPayment.orderId, Validators.required],
      paymentDate: [this.currentPayment.paymentDate, Validators.required],
      amount: [this.currentPayment.amount, [Validators.required, Validators.min(0)]],
      paymentMethod: [this.currentPayment.paymentMethod, Validators.required],
      paymentStatus: [this.currentPayment.paymentStatus, Validators.required],
      transactionId: [this.currentPayment.transactionId, Validators.required],
    });
  }

  submitForm(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.paymentForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const updatedPayment: Payment = {
      ...this.currentPayment,
      ...this.paymentForm.value
    };

    this.paymentService.updatePayment(this.currentPayment.id, updatedPayment).subscribe({
      next: () => {
        this.successMessage = 'Payment successfully updated!';
        this.resetForm();
      },
      error: () => {
        this.errorMessage = 'Error updating payment. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  markFormFieldsTouched(): void {
    Object.values(this.paymentForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm(): void {
    this.paymentForm.reset();
    this.isSubmitting = false;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
    this.dialogRef.close(true);
  }

  closeModal(): void {
    this.dialogRef.close(false);
  }
}
