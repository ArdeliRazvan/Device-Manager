import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeviceListComponent } from './features/devices/device-list/device-list.component';
import { DeviceDetailComponent } from './features/devices/device-detail/device-detail.component';
import { DeviceFormComponent } from './features/devices/device-form/device-form.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { AuthGuard } from './core/guard/auth.guard';

const routes: Routes = [
  { path: '',                 redirectTo: 'devices', pathMatch: 'full' },
  { path: 'login',            component: LoginComponent },             
  { path: 'register',         component: RegisterComponent },          
  { path: 'devices',          component: DeviceListComponent,   canActivate: [AuthGuard] },
  { path: 'devices/new',      component: DeviceFormComponent,   canActivate: [AuthGuard] },
  { path: 'devices/:id/edit', component: DeviceFormComponent,   canActivate: [AuthGuard] },
  { path: 'devices/:id',      component: DeviceDetailComponent, canActivate: [AuthGuard] },
  { path: '**',               redirectTo: 'login' }  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}