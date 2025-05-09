import {ChangeDetectorRef, Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Payment} from '../../payments/payment/payment-model';
import {PaymentsService} from '../../services/payments-service.service';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {UsersService} from '../../services/users-service.service';
import {User} from '../user/user-model';

@Component({
  selector: 'app-update-user',
  standalone: false,

  templateUrl: './update-user.component.html',
  styleUrl: './update-user.component.css'
})
export class UpdateUserComponent implements OnInit{
  userForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  currentUser: User;

  constructor(
    private fb: FormBuilder,
    private userService: UsersService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<UpdateUserComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: User }
  ) {
    this.currentUser = data.user;
  }

  ngOnInit(): void {
    console.log('Current User:', this.currentUser);

    this.userForm = this.fb.group({
      firstName: [this.currentUser.firstName, Validators.required],
      lastName: [this.currentUser.lastName, Validators.required],
      dateOfBirth: [this.formatDate(this.currentUser.dateOfBirth), [Validators.required]],
      address: [this.currentUser.address, Validators.required],
      city: [this.currentUser.city, Validators.required],
      email: [this.currentUser.email, Validators.required],
      username: [this.currentUser.username, Validators.required],
      password: [this.currentUser.password, Validators.required],
    });
  }

  submitForm(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.userForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const updateUser: Partial<User> = {
      ...this.currentUser,
      ...this.userForm.value,
      dateOfBirth: new Date(this.userForm.value.dateOfBirth).toISOString(),
      orders: [],
    };

    this.userService.updateUser(this.currentUser.id, updateUser).subscribe({
      next: () => {
        this.successMessage = 'User successfully updated!';
        this.resetForm();
      },
      error: (err) => {
        this.errorMessage = `Error updating user: ${err.error?.message || 'Unknown error'}`;
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  markFormFieldsTouched(): void {
    Object.values(this.userForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm(): void {
    this.userForm.reset();
    this.isSubmitting = false;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
    this.dialogRef.close(true);
  }

  closeModal(): void {
    this.dialogRef.close(false);
  }

  formatDate(date: any): string {
    if (!date) return '';

    let parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) return '';

    return parsedDate.toISOString().split('T')[0];
  }
}
