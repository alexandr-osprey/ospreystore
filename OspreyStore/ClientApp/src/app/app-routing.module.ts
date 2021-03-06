import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router'
import { ItemCreateUpdateComponent } from './item/item-create-update/item-create-update.component';
import { ItemThumbnailIndexComponent } from './item/item-thumbnail-index/item-thumbnail-index.component';
import { ItemDetailComponent } from './item/item-detail/item-detail.component';
import { HomepageComponent } from './homepage/homepage/homepage.component';
import { StoreCreateUpdateComponent } from './store/store-create-update/store-create-update.component';
import { StoreDetailComponent } from './store/store-detail/store-detail.component';
import { StoreIndexComponent } from './store/store-index/store-index.component';
import { IdentitySignInComponent } from './identity/identity-sign-in/identity-sign-in.component';
import { CreateUpdateAuthorizationGuard } from './guards/create-update-authorization-guard';
import { IdentitySignUpComponent } from './identity/identity-sing-up/identity-sign-up.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AdministratorAuthorizationGuard } from './guards/administrator-authorization-guard';
import { CategoryCreateUpdateComponent } from './category/category-create-update/category-create-update.component';
import { CharacteristicCreateUpdateComponent } from './characteristic/characteristic-create-update/characteristic-create-update.component';
import { CharacteristicValueCreateUpdateComponent } from './characteristic-value/characteristic-value-create-update/characteristic-value-create-update.component';
import { BrandCreateUpdateComponent } from './brand/brand-create-update/brand-create-update.component';
import { OrderCreateComponent } from './order/order-create/order-create.component';
import { OrderThumbnailIndexComponent } from './order/order-thumbnail-index/order-thumbnail-index.component';
import { SignedInAuthorizationGuard } from './guards/signed-in-authorization-guard';
import { AdministratingContentComponent } from './administrating/administrating-content/administrating-content.component';
import { CartItemThumbnailIndexComponent } from './cart/cart-item/cart-item-thumbnail-index/cart-item-thumbnail-index.component';


const routes: Routes = [

  {
    path: 'cart',
    component: CartItemThumbnailIndexComponent,
    pathMatch: 'full',
    //data: { animation: 'AboutPage' },
    data: { animation: 'cart' }
  },
  {
    path: 'administrating/content',
    component: AdministratingContentComponent,
    canActivate: [AdministratorAuthorizationGuard],
    pathMatch: 'full',
    data: { animation: 'administrating-content' },
  },
  {
    path: 'signUp',
    component: IdentitySignUpComponent,
    pathMatch: 'full',
    data: { animation: 'signUp' },
  },
  {
    path: 'signIn',
    component: IdentitySignInComponent,
    pathMatch: 'full',
    data: { animation: 'signIn' },
  },
  //{
  //  path: 'administrating',
  //  pathMatch: 'full',
  //  canActivate: [AdministratorAuthorizationGuard],
  //  component: ContentAdministratingComponent,
  //},
  {
    path: 'brands/create',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: BrandCreateUpdateComponent,
    data: { animation: 'brands-create' },
  },
  {
    path: 'brands/:id/update',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: BrandCreateUpdateComponent,
    data: { animation: 'brands-update' },
  },
  {
    path: 'categories/create',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: CategoryCreateUpdateComponent,
    data: { animation: 'categories-create' },
  },
  {
    path: 'categories/:id/update',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: CategoryCreateUpdateComponent,
    data: { animation: 'categories-update' },
  },
  {
    path: 'characteristics/create',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: CharacteristicCreateUpdateComponent,
    data: { animation: 'characteristics-create' },
  },
  {
    path: 'characteristics/:id/update',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: CharacteristicCreateUpdateComponent,
    data: { animation: 'characteristics-update' },
  },
  {
    path: 'characteristicValues/create',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: CharacteristicValueCreateUpdateComponent,
    data: { animation: 'characteristicValues-create' },
  },
  {
    path: 'characteristicValues/:id/update',
    pathMatch: 'full',
    canActivate: [AdministratorAuthorizationGuard],
    component: CharacteristicValueCreateUpdateComponent,
    data: { animation: 'characteristicValues-update' },
  },
  {
    path: 'items/create',
    pathMatch: 'full',
    canActivate: [CreateUpdateAuthorizationGuard],
    component: ItemCreateUpdateComponent,
    data: { animation: 'items-create' },
  },
  {
    path: 'items/:id/update',
    component: ItemCreateUpdateComponent,
    pathMatch: 'full',
    canActivate: [CreateUpdateAuthorizationGuard],
  },
  {
    path: 'items/:id/detail',
    component: ItemDetailComponent,
    pathMatch: 'full',
    //data: { animation: 'isLeft' }
    data: { animation: 'items-detail' },
  },
  {
    path: 'items',
    component: ItemThumbnailIndexComponent,
    //data: { animation: 'isLeft' }
    data: { animation: 'items' },
  },
  {
    path: 'stores',
    component: StoreIndexComponent,
    canActivate: [CreateUpdateAuthorizationGuard],
    data: { animation: 'stores' }
  },
  {
    path: 'orders/create',
    component: OrderCreateComponent,
    pathMatch: 'full',
    canActivate: [CreateUpdateAuthorizationGuard],
    data: { animation: 'orders-create' }
  },
  {
    path: 'orders',
    //pathMatch: 'full',
    component: OrderThumbnailIndexComponent,
    canActivate: [SignedInAuthorizationGuard],
    data: { animation: 'orders' }
  },
  {
    path: 'stores/:id/detail',
    component: StoreDetailComponent,
    data: { animation: 'stores-detail' }
  },
  {
    path: 'stores/:id/update',
    component: StoreCreateUpdateComponent,
    canActivate: [CreateUpdateAuthorizationGuard],
    data: { animation: 'stores-update' }
  },
  {
    path: 'stores/create',
    component: StoreCreateUpdateComponent,
    canActivate: [SignedInAuthorizationGuard],
    data: { animation: 'stores-create' }
  },
  {
    path: '',
    component: HomepageComponent,
    pathMatch: 'full',
  },
  { path: 'pageNotFound', component: PageNotFoundComponent },
  { path: '**', component: PageNotFoundComponent }
]
@NgModule({
  imports: [RouterModule.forRoot(routes, {
    onSameUrlNavigation: 'reload' })],
  exports: [RouterModule],
  providers: [CreateUpdateAuthorizationGuard, SignedInAuthorizationGuard, AdministratorAuthorizationGuard]
})
export class AppRoutingModule { }
