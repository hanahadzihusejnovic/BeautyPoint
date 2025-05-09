import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {AdminRoutingModule} from './admin-routing.module';
import {AdminLayoutComponent} from './admin-layout/admin-layout.component';
import {AdminErrorPageComponent} from './admin-error-page/admin-error-page.component';
import {FormsModule} from '@angular/forms';
import {SharedModule} from '../shared/shared.module';
import {MatButton} from "@angular/material/button";
import {
  MatCell,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderRow,
  MatRow,
  MatTable,
  MatTableModule
} from "@angular/material/table";
import {MatPaginator} from '@angular/material/paginator';
import {MatFormField, MatFormFieldModule} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatSortModule} from '@angular/material/sort';
import {MatIconModule} from '@angular/material/icon';
import {MatOption, MatSelect} from "@angular/material/select";
import {MatCard} from '@angular/material/card';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import {PaymentsComponent} from './payments/payment/payments.component';
import { UsersComponent } from './users/user/users.component';
import { ProductComponent } from './products/product/product.component';
import {ReservationComponent} from './reservations/reservation/reservation.component';
import {CreateProductComponent} from './products/create-product/create-proudct.component';
import { UpdateProductComponent } from './products/update-product/update-product/update-product.component';
import { CreateReservationComponent } from './reservations/create-reservation/create-reservation.component';
import { UpdateReservationComponent } from './reservations/update-reservation/update-reservation.component';
import { TreatmentComponent } from './treatments/treatment/treatment.component';
import {CreateTreatmentComponent} from './treatments/create-treatment/create-treatment.component';
import { UpdateTreatmentComponent } from './treatments/update-treatment/update-treatment.component';
import { CreatePaymentComponent } from './payments/create-payment/create-payment.component';
import { UpdatePaymentComponent } from './payments/update-payment/update-payment.component';
import { CreateUserComponent } from './users/create-user/create-user.component';
import { UpdateUserComponent } from './users/update-user/update-user.component';


@NgModule({
  declarations: [
    AdminLayoutComponent,
    ReservationComponent,
    AdminErrorPageComponent,
    AdminDashboardComponent,
    PaymentsComponent,
    UsersComponent,
    ProductComponent,
    CreateProductComponent,
    UpdateProductComponent,
    CreateReservationComponent,
    UpdateReservationComponent,
    TreatmentComponent,
    CreateTreatmentComponent,
    UpdateTreatmentComponent,
    CreatePaymentComponent,
    UpdatePaymentComponent,
    CreateUserComponent,
    UpdateUserComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    FormsModule,
    SharedModule,
    MatButton,
    MatTable,
    MatHeaderCell,
    MatCell,
    MatHeaderRow,
    MatRow,
    MatPaginator,
    MatFormField,
    MatInput,
    MatIconModule,
    MatColumnDef,
    MatTableModule,
    MatSortModule,
    MatFormFieldModule,
    MatSelect,
    MatOption,
    MatCard,
    // Omogućava pristup svemu što je eksportovano iz SharedModule
  ],
  providers: []
})
export class AdminModule {
}
