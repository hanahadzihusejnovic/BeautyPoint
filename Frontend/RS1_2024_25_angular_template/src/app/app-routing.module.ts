import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnauthorizedComponent } from './modules/shared/unauthorized/unauthorized.component';
import { AuthGuard } from './auth-guards/auth-guard.service';
import { HomeComponent } from './modules/public/home/home.component';
import { LoginComponent } from './modules/auth/login/login.component';
import { RegisterComponent } from './modules/auth/register/register.component';
import { AppointmentComponent } from './modules/auth/appointment/appointment.component';
import { BodyTreatmentComponent } from './modules/auth/body-treatment/body-treatment.component';
import { SkinTreatmentComponent } from './modules/auth/skin-treatment/skin-treatment.component';
import { HairTreatmentComponent } from './modules/auth/hair-treatment/hair-treatment.component';
import {ClientDashboardComponent} from './modules/client/client-dashboard/client-dashboard.component';
import {ProductCategoryComponent} from './modules/client/shop/product-category/product-category.component';
import {SkinCareComponent} from './modules/client/shop/product/skin-care/skin-care.component';
import {BodyCareComponent} from './modules/client/shop/product/body-care/body-care.component';
import {HairCareComponent} from './modules/client/shop/product/hair-care/hair-care.component';
import {AdminDashboardComponent} from './modules/admin/admin-dashboard/admin-dashboard.component';
import {PaymentsComponent} from './modules/admin/payments/payment/payments.component';
import {FavoritesComponent} from './modules/client/shop/product/favorites/favorites.component';
import {BodyProductDetailsComponent} from './modules/client/shop/product/body-care/body-product-details/body-product-details.component';
import {HairProductDetailsComponent} from './modules/client/shop/product/hair-care/hair-product-details/hair-product-details.component';
import {SkinProductDetailsComponent} from './modules/client/shop/product/skin-care/skin-product-details/skin-product-details.component';
import {LogoutComponent} from './modules/auth/logout/logout.component';
import {ShoppingCartComponent} from './modules/client/shop/product/shopping-cart/shopping-cart.component';
import {SaveForLaterComponent} from './modules/client/shop/product/save-for-later/save-for-later.component';


const routes: Routes = [
  { path: 'unauthorized', component: UnauthorizedComponent },

  {
    path: 'admin',
    canActivate: [AuthGuard],
    data: { isAdmin: true },
    loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule)  // Lazy load admin module
  },
  {
    path: 'employee',
    canActivate: [AuthGuard],
    data: { isEmployee: true },
    loadChildren: () => import('./modules/employee/employee.module').then(m => m.EmployeeModule)  // Lazy load employee module
  },

  {
    path: 'client',
    canActivate: [AuthGuard],
    data: { isClient: true },
    loadChildren: () => import('./modules/client/client.module').then(m => m.ClientModule)  // Lazy load client module
  },

  // Public module
  {
    path: 'public',
    loadChildren: () => import('./modules/public/public.module').then(m => m.PublicModule)  // Lazy load public module
  },

  // Authentication related routes
  {
    path: 'auth',
    loadChildren: () => import('./modules/auth/auth.module').then(m => m.AuthModule)  // Lazy load auth module
  },

  // Other routes
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'appointment',
    component: AppointmentComponent
  },
  {
    path: 'body-treatment',
    component: BodyTreatmentComponent
  },
  {
    path: 'skin-treatment',
    component: SkinTreatmentComponent
  },
  {
    path: 'hair-treatment',
    component: HairTreatmentComponent
  },
  {
    path: 'client-dashboard',
    component: ClientDashboardComponent
  },
  {
    path: 'product-category',
    component: ProductCategoryComponent
  },
  {
    path: 'skin-care',
    component: SkinCareComponent
  },
  {
    path: 'body-care',
    component: BodyCareComponent
  },
  {
    path: 'hair-care',
    component: HairCareComponent
  },
  {
    path: 'admin-dashboard',
    component: AdminDashboardComponent
  },
  {
    path: 'favorites',
    component: FavoritesComponent
  },
  {
    path: 'body-product-details/:id',
    component: BodyProductDetailsComponent
  },
  {
    path: 'hair-product-details/:id',
    component: HairProductDetailsComponent
  },
  {
    path: 'skin-product-details/:id',
    component: SkinProductDetailsComponent
  },
  {
    path: 'logout',
    component: LogoutComponent
  },
  {
    path: 'shopping-cart',
    component: ShoppingCartComponent
  },
  {
    path: 'save-for-later',
    component: SaveForLaterComponent
  },
  { path: 'employee', loadChildren: () => import('./modules/employee/employee.module').then(m => m.EmployeeModule) },

  // Default route if no matching route is found
  { path: '', redirectTo: 'public', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule{}
