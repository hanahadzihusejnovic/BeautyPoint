import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReservationService } from '../../services/reservations-service.service';
import { MatDialogRef } from '@angular/material/dialog';
import { User } from '../reservation/user-model';
import { Service } from '../reservation/service-model';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-create-reservation',
  templateUrl: './create-reservation.component.html',
  standalone: false,
  styleUrls: ['./create-reservation.component.css']
})
export class CreateReservationComponent implements OnInit {
  reservationForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  selectedFile: File | null = null;
  users: User[] = [];
  services: Service[] = [];

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<CreateReservationComponent>
  ) {}

  ngOnInit() {
    this.reservationForm = this.fb.group({
      userId: ['', Validators.required],
      serviceId: ['', Validators.required],
      reservationDate: ['', Validators.required],
      reservationStatus: ['', Validators.required],
    });

    this.loadUsers();
    this.loadServices();
  }

  loadUsers() {
    this.reservationService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        console.log('Fetched users:', this.users);
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching users:', error),
    });
  }


  loadServices() {
    this.reservationService.getServices().subscribe({
      next: (services) => {
        this.services = services;
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching services:', error),
    });
  }

  submitForm() {
    this.successMessage = '';
    this.errorMessage = '';

    console.log("Form values: ", this.reservationForm.value);
    console.log("UserId: ", this.reservationForm.value.userId);

    if (this.reservationForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const selectedUserId = this.reservationForm.value.userId;
    const selectedUser = this.users.find(user => user.id === selectedUserId);

    const reservationData = {
      userId: selectedUser ? selectedUser.id : '',
      serviceId: this.reservationForm.value.serviceId,
      reservationDate: this.reservationForm.value.reservationDate,
      reservationStatus: this.reservationForm.value.reservationStatus,
      userFirstName: selectedUser ? selectedUser.firstName : 'Unknown First Name',
      userLastName: selectedUser ? selectedUser.lastName : 'Unknown Last Name',
      serviceName: this.getServiceNameById(this.reservationForm.value.serviceId),
    };

    console.log('Final JSON before sending:', reservationData);


    this.reservationService.createReservation(reservationData).subscribe({
      next: (response) => {
        console.log('Reservation created successfully:', response);
        this.successMessage = 'Reservation successfully created!';
        this.resetForm();
      },
      error: (error) => {
        console.error('Error creating reservation:', error);
        this.errorMessage = 'Error creating reservation. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  getServiceNameById(serviceId: any): string {
    const service = this.services.find(s => s.id == serviceId);
    return service ? service.serviceName : 'Unknown Service';
  }

  markFormFieldsTouched() {
    Object.values(this.reservationForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm() {
    this.reservationForm.reset();
    this.selectedFile = null;
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
