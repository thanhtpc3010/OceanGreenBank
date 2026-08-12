import { Routes } from '@angular/router';

import { authGuard } from './core/auth/auth.guard';
import { LayoutComponent } from './layout/layout.component';
import { AccountComponent } from './pages/account/account.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { EditAccountComponent } from './pages/account/edit-account.component';
import { LoginComponent } from './pages/login/login.component';
import { PasswordAccountComponent } from './pages/account/password-account.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      {
        path: 'account',
        children: [
          { path: '', component: AccountComponent },
          { path: 'edit', component: EditAccountComponent },
          { path: 'password', component: PasswordAccountComponent },
        ],
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
