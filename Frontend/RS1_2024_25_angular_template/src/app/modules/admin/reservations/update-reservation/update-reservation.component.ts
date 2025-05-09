import { Component, ChangeDetectorRef, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReservationService } from '../../services/reservations-service.service';
import { Reservation } from '../reservation/reservation-model';
import { User } from '../reservation/user-model';
import { Service } from '../reservation/service-model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MyConfig } from '../../../../my-config';

@Component({
  selector: 'app-update-reservation',
  templateUrl: './update-reservation.component.html',
  standalone: false,
  styleUrls: ['./update-reservation.component.css']
})
export class UpdateReservationComponent implements OnInit {
  reservationForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  currentReservation: Reservation;
  users: User[] = [];
  services: Service[] = [];

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<UpdateReservationComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { reservation: Reservation }
  ) {
    this.currentReservation = data.reservation;
  }

  ngOnInit(): void {
    this.reservationForm = this.fb.group({
      userId: [this.currentReservation.userId, Validators.required],
      userFirstName: [this.currentReservation.userFirstName, Validators.required],
      userLastName: [this.currentReservation.userLastName, Validators.required],
      serviceId: [this.currentReservation.serviceId, Validators.required],
      serviceName: [this.currentReservation.serviceName, Validators.required],
      reservationDate: [this.currentReservation.reservationDate, Validators.required],
      reservationStatus: [this.currentReservation.reservationStatus, Validators.required],
    });

    this.loadUsers();
    this.loadServices();
  }

  loadUsers(): void {
    this.reservationService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching users:', error)
    });
  }

  loadServices(): void {
    this.reservationService.getServices().subscribe({
      next: (services) => {
        this.services = services;
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching services:', error)
    });
  }

  submitForm(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.reservationForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const updatedReservation: Reservation = {
      ...this.currentReservation,
      ...this.reservationForm.value
    };

    this.reservationService.updateReservation(this.currentReservation.id, updatedReservation).subscribe({
      next: (response) => {
        this.successMessage = 'Reservation successfully updated!';
        this.resetForm();
      },
      error: (error) => {
        this.errorMessage = 'Error updating reservation. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  markFormFieldsTouched(): void {
    Object.values(this.reservationForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm(): void {
    this.reservationForm.reset();
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
