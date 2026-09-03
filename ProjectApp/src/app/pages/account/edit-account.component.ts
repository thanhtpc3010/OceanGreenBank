import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../core/services/user.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-account-edit',
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './edit-account.component.html',
  styleUrl: './edit-account.component.scss',
})
export class EditAccountComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');

  /* ---- Form (chỉ các field được phép sửa) ---- */
  protected readonly fullName = signal('');
  protected readonly phone = signal('');
  protected readonly gender = signal('Nam');
  protected readonly address = signal('');

  /* ---- Read-only ---- */
  protected readonly email = signal('');
  protected readonly identityCard = signal('');
  protected readonly dateOfBirth = signal('');

  async ngOnInit(): Promise<void> {
    const p = await this.userService.getProfile();
    this.fullName.set(p.fullName);
    this.phone.set(p.phone);
    this.gender.set(p.gender);
    this.address.set(p.address);
    this.email.set(p.email);
    this.identityCard.set(p.identityCard);
    this.dateOfBirth.set(p.dateOfBirth);
    this.loading.set(false);
  }

  protected async save(): Promise<void> {
    this.error.set('');
    this.success.set('');

    if (!this.fullName().trim() || !this.phone().trim()) {
      this.error.set(this.language.t('EDIT.ERR_REQUIRED'));
      return;
    }

    this.saving.set(true);
    try {
      await this.userService.updateProfile({
        fullName: this.fullName().trim(),
        phone: this.phone().trim(),
        gender: this.gender(),
        address: this.address().trim(),
      });
      this.success.set(this.language.t('EDIT.SUCCESS'));
      setTimeout(() => this.router.navigate(['/account']), 900);
    } finally {
      this.saving.set(false);
    }
  }
}
