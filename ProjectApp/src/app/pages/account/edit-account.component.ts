import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-account-edit',
  imports: [FormsModule, RouterLink],
  templateUrl: './edit-account.component.html',
  styleUrl: './edit-account.component.scss',
})
export class EditAccountComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

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
      this.error.set('Họ tên và số điện thoại không được để trống.');
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
      this.success.set('Cập nhật thông tin thành công!');
      setTimeout(() => this.router.navigate(['/account']), 900);
    } finally {
      this.saving.set(false);
    }
  }
}
