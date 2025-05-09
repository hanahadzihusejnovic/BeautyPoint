import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminLayoutComponent} from './admin-layout/admin-layout.component';
import {AdminErrorPageComponent} from './admin-error-page/admin-error-page.component';
import {PaymentsComponent} from './payments/payment/payments.component';
import {UsersComponent} from './users/user/users.component';
import {ProductComponent} from './products/product/product.component';
import {ReservationComponent} from './reservations/reservation/reservation.component';
import {TreatmentComponent} from './treatments/treatment/treatment.component';

const routes: Routes = [
  {
    path: 'payments',
    component: PaymentsComponent
  },
  {
    path: 'users',
    component: UsersComponent
  },
  {
    path: 'products',
    component: ProductComponent
  },
  {
   path: 'reservations',
   component: ReservationComponent
  },
  {
    path:'treatments',
    component: TreatmentComponent
  },
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
      {path: 'order', component: ReservationComponent},
      {path: '**', component: AdminErrorPageComponent},
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule {
}
