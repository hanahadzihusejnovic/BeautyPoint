/*import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ReservationComponent} from './reservation/reservation.component';

const routes: Routes = [
  {path: 'reservation', component: ReservationComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule {
} */

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReservationComponent } from './reservation/reservation.component';
import { ProductCategoryComponent } from './shop/product-category/product-category.component';
import { SkinCareComponent } from './shop/product/skin-care/skin-care.component';
import { BodyCareComponent } from './shop/product/body-care/body-care.component';
import { HairCareComponent } from './shop/product/hair-care/hair-care.component';
import { FavoritesComponent } from './shop/product/favorites/favorites.component';
import {BodyProductDetailsComponent} from './shop/product/body-care/body-product-details/body-product-details.component';
import {HairProductDetailsComponent} from './shop/product/hair-care/hair-product-details/hair-product-details.component';
import {
  SkinProductDetailsComponent
} from './shop/product/skin-care/skin-product-details/skin-product-details.component';
import {ShoppingCartComponent} from './shop/product/shopping-cart/shopping-cart.component';
import {SaveForLaterComponent} from './shop/product/save-for-later/save-for-later.component';

const routes: Routes = [
  { path: 'reservation', component: ReservationComponent },
  { path: 'product-category', component: ProductCategoryComponent },
  { path: 'skin-care', component: SkinCareComponent },
  { path: 'body-care', component: BodyCareComponent },
  { path: 'hair-care', component: HairCareComponent },
  { path: 'favorites', component: FavoritesComponent },
  { path: 'body-product-details/:id', component: BodyProductDetailsComponent },
  { path: 'hair-product-details/:id', component: HairProductDetailsComponent },
  { path: 'skin-product-details/:id', component: SkinProductDetailsComponent },
  { path: 'shopping-cart', component: ShoppingCartComponent },
  { path: 'save-for-later', component: SaveForLaterComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule { }

