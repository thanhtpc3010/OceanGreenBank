import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { ConfirmDialogComponent } from './shared/confirm-dialog/confirm-dialog.component';
import { TransactionPasswordDialogComponent } from './shared/transaction-password-dialog/transaction-password-dialog.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ConfirmDialogComponent, TransactionPasswordDialogComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {}
