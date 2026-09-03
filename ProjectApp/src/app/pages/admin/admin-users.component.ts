import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ConfirmService } from '../../core/services/confirm.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { UserService } from '../../core/services/user.service';

interface AdminUser {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  identityCard: string;
  dateOfBirth: string;
  address: string | null;
  isActive: boolean;
  createdDate: string;
}

interface UserPermView {
  fullName: string;
  email: string;
  roles: { code: string | null; roleName: string }[];
  permissions: string[];
}

@Component({
  selector: 'app-admin-users',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly confirmService = inject(ConfirmService);
  private readonly language = inject(LanguageService);

  protected readonly users = signal<AdminUser[]>([]);
  protected readonly loading = signal(true);
  protected readonly search = signal('');
  protected readonly error = signal('');

  /* ---- Thêm user ---- */
  protected readonly showAddForm = signal(false);
  protected readonly adding = signal(false);
  protected readonly newUser = {
    fullName: '',
    email: '',
    phone: '',
    identityCard: '',
    dateOfBirth: '',
    address: '',
    password: '',
  };

  /* ---- Modal xem quyền ---- */
  protected readonly permView = signal<UserPermView | null>(null);
  protected readonly permLoading = signal(false);

  protected readonly filteredUsers = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.users();
    return this.users().filter(
      (u) => u.fullName.toLowerCase().includes(q) || u.email.toLowerCase().includes(q),
    );
  });

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  private async reload(): Promise<void> {
    this.loading.set(true);
    try {
      const list = await this.userService.getAllUsers();
      this.users.set(list);
    } finally {
      this.loading.set(false);
    }
  }

  protected formatDate(iso: string): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime()) ? iso : d.toLocaleDateString('vi-VN');
  }

  protected async addUser(): Promise<void> {
    this.error.set('');
    const u = this.newUser;
    if (!u.fullName.trim() || !u.email.trim() || !u.password.trim()) {
      this.error.set(this.language.t('ADMIN.USERS_ERR_REQUIRED'));
      return;
    }
    this.adding.set(true);
    try {
      await this.userService.createUserByAdmin({
        fullName: u.fullName.trim(),
        email: u.email.trim(),
        phone: u.phone.trim() || '0900000000',
        identityCard: u.identityCard.trim() || '000000000000',
        dateOfBirth: u.dateOfBirth || '2000-01-01',
        password: u.password,
        address: u.address.trim() || undefined,
      });
      this.showAddForm.set(false);
      Object.assign(this.newUser, { fullName: '', email: '', phone: '', identityCard: '', dateOfBirth: '', address: '', password: '' });
      await this.reload();
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.adding.set(false);
    }
  }

  protected async toggleActive(user: AdminUser): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: this.language.t(user.isActive ? 'ADMIN.USERS_LOCK_TITLE' : 'ADMIN.USERS_UNLOCK_TITLE'),
      message: this.language.t(user.isActive ? 'ADMIN.USERS_LOCK_MSG' : 'ADMIN.USERS_UNLOCK_MSG', { name: user.fullName }),
      confirmText: this.language.t(user.isActive ? 'ADMIN.USERS_LOCK' : 'ADMIN.USERS_UNLOCK'),
      danger: user.isActive,
    });
    if (!ok) return;
    try {
      await this.userService.updateUserByAdmin(user.id, { isActive: !user.isActive });
      await this.reload();
    } catch (e) {
      this.error.set(this.extractError(e));
    }
  }

  protected async deleteUser(user: AdminUser): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: this.language.t('ADMIN.USERS_DELETE_TITLE'),
      message: this.language.t('ADMIN.USERS_DELETE_MSG', { name: user.fullName, email: user.email }),
      confirmText: this.language.t('COMMON.DELETE'),
      danger: true,
    });
    if (!ok) return;
    try {
      await this.userService.deleteUserById(user.id);
      await this.reload();
    } catch (e) {
      this.error.set(this.extractError(e));
    }
  }

  protected async openPermissions(user: AdminUser): Promise<void> {
    this.permLoading.set(true);
    this.permView.set(null);
    try {
      const data = await this.userService.getUserPermissions(user.id);
      this.permView.set({
        fullName: data.fullName,
        email: data.email,
        roles: data.roles.map((r) => ({ code: r.code, roleName: r.roleName })),
        permissions: data.permissionCodes,
      });
    } finally {
      this.permLoading.set(false);
    }
  }

  protected closePermissions(): void {
    this.permView.set(null);
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('ADMIN.USERS_ERR_DEFAULT'));
  }
}
