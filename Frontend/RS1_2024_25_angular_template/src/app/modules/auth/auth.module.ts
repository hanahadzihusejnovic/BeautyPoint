import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {AuthRoutingModule} from './auth-routing.module';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {TwoFactorComponent} from './two-factor/two-factor.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {LogoutComponent} from './logout/logout.component';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {MatButton} from "@angular/material/button";
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {TranslatePipe} from "@ngx-translate/core";
import {SharedModule} from "../shared/shared.module";


@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    TwoFactorComponent,
    LogoutComponent,
    AuthLayoutComponent,
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    FormsModule,
    MatButton,
    MatSlideToggle,
    TranslatePipe,
    SharedModule,
    ReactiveFormsModule

  ]
})
export class AuthModule {
}
