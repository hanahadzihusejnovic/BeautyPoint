import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientRoutingModule } from './client-routing.module';
import { ReservationComponent } from './reservation/reservation.component';
import { ClientDashboardComponent } from './client-dashboard/client-dashboard.component';
import { ProductCategoryComponent } from './shop/product-category/product-category.component';
import { SkinCareComponent } from './shop/product/skin-care/skin-care.component';
import { BodyCareComponent } from './shop/product/body-care/body-care.component';
import { HairCareComponent } from './shop/product/hair-care/hair-care.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { FavoritesComponent } from './shop/product/favorites/favorites.component';
import {CdkDrag} from "@angular/cdk/drag-drop";
import { BodyProductDetailsComponent } from './shop/product/body-care/body-product-details/body-product-details.component';
import { HairProductDetailsComponent } from './shop/product/hair-care/hair-product-details/hair-product-details.component';
import { SkinProductDetailsComponent } from './shop/product/skin-care/skin-product-details/skin-product-details.component';
import {MatMenu, MatMenuTrigger} from "@angular/material/menu";
import {MatIconButton} from '@angular/material/button';
import {MatIcon, MatIconModule} from '@angular/material/icon';
import {MatFormField} from '@angular/material/form-field';
import { ShoppingCartComponent } from './shop/product/shopping-cart/shopping-cart.component';
import { SaveForLaterComponent } from './shop/product/save-for-later/save-for-later.component';


@NgModule({
  declarations: [
    ReservationComponent,
    ClientDashboardComponent,
    ProductCategoryComponent,
    SkinCareComponent,
    BodyCareComponent,
    HairCareComponent,
    FavoritesComponent,
    BodyProductDetailsComponent,
    HairProductDetailsComponent,
    SkinProductDetailsComponent,
    ShoppingCartComponent,
    SaveForLaterComponent,
  ],
  imports: [
    CommonModule,
    ClientRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    CdkDrag,
    MatMenuTrigger,
    MatIconButton,
    MatMenu,
    MatIcon,
    MatFormField,
    MatIconModule,
  ]
})
export class ClientModule { }
