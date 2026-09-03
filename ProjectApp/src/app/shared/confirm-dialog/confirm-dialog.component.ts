import { Component, inject } from '@angular/core';

import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { ConfirmService } from '../../core/services/confirm.service';

@Component({
  selector: 'app-confirm-dialog',
  imports: [TranslatePipe],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.scss',
})
export class ConfirmDialogComponent {
  private readonly confirmService = inject(ConfirmService);

  protected readonly state = this.confirmService.state;

  protected cancel(): void {
    this.confirmService.resolve(false);
  }

  protected confirm(): void {
    this.confirmService.resolve(true);
  }
}
