import { Routes } from '@angular/router';

import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/admin.guard';
import { LayoutComponent } from './layout/layout.component';
import { AccountComponent } from './pages/account/account.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { EditAccountComponent } from './pages/account/edit-account.component';
import { LoginComponent } from './pages/login/login.component';
import { PasswordAccountComponent } from './pages/account/password-account.component';
import { TransferComponent } from './pages/transfer/transfer.component';
import { TransactionsComponent } from './pages/transactions/transactions.component';
import { SavingsComponent } from './pages/savings/savings.component';
import { DepositComponent } from './pages/deposit/deposit.component';
import { WalletPayComponent } from './pages/wallet-pay/wallet-pay.component';
import { AdminUsersComponent } from './pages/admin/admin-users.component';
import { AdminAutoEarnComponent } from './pages/admin/admin-auto-earn.component';
import { AdminKnowledgeComponent } from './pages/admin/admin-knowledge.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  // Trang ví mô phỏng MoMo/ZaloPay — đứng ngoài layout SmartBank (như redirect sang app ví).
  {
    path: 'wallet-pay/:id',
    component: WalletPayComponent,
    canActivate: [authGuard],
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'transfer', component: TransferComponent },
      { path: 'transactions', component: TransactionsComponent },
      { path: 'savings', component: SavingsComponent },
      { path: 'deposit', component: DepositComponent },
      { path: 'admin/users', component: AdminUsersComponent, canActivate: [adminGuard] },
      { path: 'admin/auto-earn', component: AdminAutoEarnComponent, canActivate: [adminGuard] },
      { path: 'admin/knowledge', component: AdminKnowledgeComponent, canActivate: [adminGuard] },
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
