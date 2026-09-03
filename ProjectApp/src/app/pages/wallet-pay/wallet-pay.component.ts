import { DecimalPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { PaymentService, PaymentProvider, PaymentDto } from '../../core/services/payment.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-wallet-pay',
  imports: [DecimalPipe, DatePipe, TranslatePipe],
  templateUrl: './wallet-pay.component.html',
  styleUrl: './wallet-pay.component.scss',
})
export class WalletPayComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly paymentService = inject(PaymentService);
  private readonly language = inject(LanguageService);

  protected readonly payment = signal<PaymentDto | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly acting = signal(false);

  protected readonly isMomo = computed(() => this.payment()?.provider === PaymentProvider.Momo);
  protected readonly providerColor = computed(() => (this.isMomo() ? '#d82d8b' : '#0068ff'));

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set(this.language.t('WALLET.ERR_NOT_FOUND'));
      this.loading.set(false);
      return;
    }
    try {
      this.payment.set(await this.paymentService.getPayment(id));
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.loading.set(false);
    }
  }

  protected async confirm(): Promise<void> {
    const p = this.payment();
    if (!p) return;
    this.acting.set(true);
    this.error.set('');
    try {
      this.payment.set(await this.paymentService.confirmPayment(p.id));
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.acting.set(false);
    }
  }

  protected async cancel(): Promise<void> {
    const p = this.payment();
    if (!p) return;
    this.acting.set(true);
    this.error.set('');
    try {
      this.payment.set(await this.paymentService.cancelPayment(p.id));
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.acting.set(false);
    }
  }

  protected providerLabel(): string {
    return this.payment()?.providerName ?? this.language.t('WALLET.PROVIDER_FALLBACK');
  }

  protected backToDeposit(): void {
    this.router.navigate(['/deposit']);
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('WALLET.ERR_DEFAULT'));
  }
}
