import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {TreatmentService} from '../../services/treatment-service.service';
import {MatDialogRef} from '@angular/material/dialog';
import {PaymentsService} from '../../services/payments-service.service';

@Component({
  selector: 'app-create-payment',
  standalone: false,

  templateUrl: './create-payment.component.html',
  styleUrl: './create-payment.component.css'
})
export class CreatePaymentComponent implements OnInit{
  paymentForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private paymentService: PaymentsService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<CreatePaymentComponent>
  ) {}

  paymentMethods = ['Card', 'Cash On Delivery'];
  paymentStatuses = ['Pending', 'Completed', 'Failed'];

  ngOnInit() {
    this.paymentForm = this.fb.group({
      orderId: ['', Validators.required],
      paymentDate: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      paymentMethod: ['', Validators.required],
      paymentStatus: ['', Validators.required],
      transactionId: ['', Validators.required]
    });
  }

  submitForm() {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.paymentForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const paymentData = {
      orderId: this.paymentForm.value.orderId,
      paymentDate: this.paymentForm.value.paymentDate,
      amount: this.paymentForm.value.amount,
      paymentMethod: this.paymentForm.value.paymentMethod,
      paymentStatus: this.paymentForm.value.paymentStatus,
      transactionId: this.paymentForm.value.transactionId.toString()
    };

    console.log('JSON to be sent:', paymentData);

    this.paymentService.createPayment(paymentData).subscribe({
      next: () => {
        this.successMessage = 'Payment successfully created!';
        this.resetForm();
      },
      error: () => {
        this.errorMessage = 'Error creating payment. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  markFormFieldsTouched() {
    Object.values(this.paymentForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm() {
    this.paymentForm.reset();
    this.isSubmitting = false;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
    this.dialogRef.close(true);
  }

  closeModal() {
    this.dialogRef.close(false);
  }

}
