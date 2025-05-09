import {Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { AuthRegisterEndpointService } from '../../../endpoints/auth-endpoints/auth-register-endpoint.service';
import {AbstractControl, FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MySnackbarHelperService } from '../../../modules/shared/snackbars/my-snackbar-helper.service';
import {TranslateService} from '@ngx-translate/core';
import {usernameExistsValidator} from './username.validator';
import {UserService} from '../../../services/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: false,
})
export class RegisterComponent implements OnInit{
  form!: FormGroup;
  successMessage = '';
  errorMessage = '';
  isSubmitting = false;

  languages = [
    { code: 'en', label: 'English' },
    { code: 'bs', label: 'Bosanski' }
  ];

  currentLanguage: string = 'en';

  constructor(
    private fb: FormBuilder,
    private authRegisterService: AuthRegisterEndpointService,
    private router: Router,
    private snackBar: MySnackbarHelperService,
    private translate: TranslateService,
    private userService: UserService,
  ) {
    this.currentLanguage = localStorage.getItem('appLanguage') || 'en';

  }
  ngOnInit(): void {
    const savedLang = localStorage.getItem('appLanguage') || 'en';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
    this.currentLanguage = savedLang;

    this.form = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required]],
      city: ['', [Validators.required]],
      address: ['', [Validators.required]],
      username: ['', {
        validators: [Validators.required],
        asyncValidators: [usernameExistsValidator(this.userService)],
        updateOn: 'blur'
      }],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]],
      dateOfBirth: ['', [Validators.required]]
    });
  }

  changeLanguage(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const lang = selectElement.value;
    this.translate.use(lang);
    localStorage.setItem('appLanguage', lang);
  }

  get firstName(): AbstractControl {
    return this.form.get('firstName')!;
  }

  get lastName(): AbstractControl {
    return this.form.get('lastName')!;
  }

  get address(): AbstractControl {
    return this.form.get('address')!;
  }

  get email(): AbstractControl {
    return this.form.get('email')!;
  }

  get city(): AbstractControl {
    return this.form.get('city')!;
  }

  get dateOfBirth(): AbstractControl {
    return this.form.get('dateOfBirth')!;
  }

  get username(): AbstractControl {
    return this.form.get('username')!;
  }

  get password(): AbstractControl {
    return this.form.get('password')!;
  }

  markFormFieldsTouched() {
    Object.values(this.form.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  onRegister(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.isSubmitting = false;

    if (this.form.invalid) {
      this.markFormFieldsTouched();
      return;
    }

    const registerData = {
      firstName: this.form.value.firstName,
      lastName: this.form.value.lastName,
      email: this.form.value.email,
      city: this.form.value.city,
      address: this.form.value.address,
      phone: this.form.value.phone,
      username: this.form.value.username,
      password: this.form.value.password,
      dateOfBirth: this.form.value.dateOfBirth
    };

    this.authRegisterService.register(registerData).subscribe({
      next: () => {
        this.snackBar.showMessage('Registration successful! Redirecting to login...');
        this.router.navigate(['/login']);
        this.isSubmitting = false;
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Registration failed:', err);
        this.snackBar.showMessage('Registration failed. Please try again.');
      }
    });
  }
}
