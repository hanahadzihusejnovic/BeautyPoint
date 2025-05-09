import {Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { AuthLoginEndpointService } from '../../../endpoints/auth-endpoints/auth-login-endpoint.service';
import {AbstractControl, FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MyInputTextType } from '../../shared/my-reactive-forms/my-input-text/my-input-text.component';
import {MyAuthService} from '../../../services/auth-services/my-auth.service';
import {LoginTokenDto} from '../../../services/auth-services/dto/login-token-dto';
import {TranslateService} from '@ngx-translate/core';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: false,
})
export class LoginComponent implements OnInit{
  form!: FormGroup;
  successMessage = '';
  errorMessage = '';
  invalidErrorMessage = '';
  isSubmitting = false;
  protected readonly MyInputTextType = MyInputTextType;

  languages = [
    { code: 'en', label: 'English' },
    { code: 'bs', label: 'Bosanski' }
  ];

  currentLanguage: string = 'en';

  constructor(
    private fb: FormBuilder,
    private authLoginService: AuthLoginEndpointService,
    private authService: MyAuthService,
    private router: Router,
    private translate: TranslateService,
    private snack: MatSnackBar
  ) {
    this.currentLanguage = localStorage.getItem('appLanguage') || 'en';

  }

  ngOnInit(): void {
    const savedLang = localStorage.getItem('appLanguage') || 'en';
    this.translate.setDefaultLang(savedLang);
    this.translate.use(savedLang);
    this.currentLanguage = savedLang;

    this.form = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]],
    });
  }

  changeLanguage(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const lang = selectElement.value;
    this.translate.use(lang);
    localStorage.setItem('appLanguage', lang);
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

  showError(message: string) {
    this.snack.open(message, 'OK', {
      duration: 4000,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }

  onLogin(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.isSubmitting = false;

    if (this.form.invalid) {
      this.markFormFieldsTouched();
      return;
    }
    this.authLoginService.handleAsync(this.form.value).subscribe({
      next: (response: LoginTokenDto) => {
        this.isSubmitting = false;

        if (response.accessToken && response.myAuthInfo) {
          this.authService.setLoggedInUser(response);

          switch (response.myAuthInfo.role) {
            case 'Admin':
              this.router.navigate(['/admin-dashboard']);
              break;
            case 'Client':
              this.router.navigate(['/client-dashboard']);
              break;
            case 'Employee':
              this.router.navigate(['/body-treatment']);
              break;
            default:
              console.error('Unknown role');
          }
        } else {
          console.error('Invalid login response.');
        }
      },
      error: (err) => {
        this.isSubmitting = false;
        if (err.status === 401) {
          this.showError(this.translate.instant('invalidLogin'));
        } else {
          this.showError(this.translate.instant('unexpectedError'));
        }

      },
    });
  }
}
