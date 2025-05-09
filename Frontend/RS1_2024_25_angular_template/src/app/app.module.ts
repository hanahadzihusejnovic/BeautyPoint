import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HTTP_INTERCEPTORS, HttpClient, HttpClientModule} from '@angular/common/http';
import {MyAuthInterceptor} from './services/auth-services/my-auth-interceptor.service';
import {MyAuthService} from './services/auth-services/my-auth.service';
import {SharedModule} from './modules/shared/shared.module';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MyErrorHandlingInterceptor} from './services/auth-services/my-error-handling-interceptor.service';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatOption, MatSelect} from '@angular/material/select';
import {CustomTranslateLoader} from './services/custom-translate-loader';
import { IntroPageComponent } from './modules/auth/intro-page/intro-page.component';
import { AppointmentComponent } from './modules/auth/appointment/appointment.component';
import { BodyTreatmentComponent } from './modules/auth/body-treatment/body-treatment.component';
import { SkinTreatmentComponent } from './modules/auth/skin-treatment/skin-treatment.component';
import { HairTreatmentComponent } from './modules/auth/hair-treatment/hair-treatment.component';
import {FormsModule} from '@angular/forms';
import {DragDropModule} from '@angular/cdk/drag-drop';
import {MatInputModule} from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule } from '@angular/material/dialog';
import {MatIconModule} from '@angular/material/icon';
import { NgxStripeModule } from 'ngx-stripe';
import {environment} from '../environment';
import {MatSnackBar, MatSnackBarModule} from '@angular/material/snack-bar';


@NgModule({
  declarations: [
    AppComponent,
    IntroPageComponent,
    AppointmentComponent,
    BodyTreatmentComponent,
    SkinTreatmentComponent,
    HairTreatmentComponent,
  ],
  imports: [
    BrowserAnimationsModule, // Potrebno za animacije
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    SharedModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (http: HttpClient) => new CustomTranslateLoader(http),
        deps: [HttpClient]
      }
    }),
    FormsModule,
    MatFormField,
    MatSelect,
    MatOption,
    MatLabel,
    DragDropModule,
    MatInputModule,
    HttpClientModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatSnackBarModule,
    NgxStripeModule.forRoot(environment.stripePublishableKey)
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MyAuthInterceptor,
      multi: true // Ensures multiple interceptors can be used if needed
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MyErrorHandlingInterceptor,
      multi: true // Dodaje ErrorHandlingInterceptor u lanac
    },
    MyAuthService,
    provideAnimationsAsync() // Ensure MyAuthService is available for the interceptor
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
