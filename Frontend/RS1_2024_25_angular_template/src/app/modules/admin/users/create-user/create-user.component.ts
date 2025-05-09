import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {PaymentsService} from '../../services/payments-service.service';
import {MatDialogRef} from '@angular/material/dialog';
import {UsersService} from '../../services/users-service.service';

@Component({
  selector: 'app-create-user',
  standalone: false,

  templateUrl: './create-user.component.html',
  styleUrl: './create-user.component.css'
})
export class CreateUserComponent implements OnInit {
  userForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private userService: UsersService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<CreateUserComponent>
  ) {}

  ngOnInit() {
    this.userForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      email: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  submitForm() {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.userForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const userData = {
      firstName: this.userForm.value.firstName,
      lastName: this.userForm.value.lastName,
      dateOfBirth: this.userForm.value.dateOfBirth,
      address: this.userForm.value.address,
      city: this.userForm.value.city,
      email: this.userForm.value.email,
      username: this.userForm.value.username,
      password: this.userForm.value.password
    };

    console.log('JSON to be sent:', userData);

    this.userService.createUser(userData).subscribe({
      next: () => {
        this.successMessage = 'User successfully created!';
        this.resetForm();
      },
      error: () => {
        this.errorMessage = 'Error creating user. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  markFormFieldsTouched() {
    Object.values(this.userForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm() {
    this.userForm.reset();
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
