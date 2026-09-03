import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ConfirmService } from '../../core/services/confirm.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TranslationService, TranslationItem } from '../../core/services/translation.service';

@Component({
  selector: 'app-admin-translations',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './admin-translations.component.html',
  styleUrl: './admin-translations.component.scss',
})
export class AdminTranslationsComponent implements OnInit {
  private readonly translationService = inject(TranslationService);
  private readonly confirmService = inject(ConfirmService);
  private readonly language = inject(LanguageService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly items = signal<TranslationItem[]>([]);

  /* ---- Lọc ---- */
  protected readonly search = signal('');
  protected readonly langFilter = signal<'all' | 'vi' | 'en'>('all');

  /* ---- Modal thêm / sửa ---- */
  protected readonly showModal = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly formKey = signal('');
  protected readonly formLanguage = signal('vi');
  protected readonly formValue = signal('');

  protected readonly filtered = computed(() => {
    const q = this.search().trim().toLowerCase();
    const lang = this.langFilter();
    return this.items().filter((t) => {
      const matchesLang = lang === 'all' || t.language === lang;
      const matchesQuery =
        !q || t.key.toLowerCase().includes(q) || t.value.toLowerCase().includes(q);
      return matchesLang && matchesQuery;
    });
  });

  async ngOnInit(): Promise<void> {
    await this.load();
  }

  protected async load(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      this.items.set(await this.translationService.getAll());
    } catch (e) {
      this.error.set(this.extractMessage(e));
    } finally {
      this.loading.set(false);
    }
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.formKey.set('');
    this.formLanguage.set('vi');
    this.formValue.set('');
    this.showModal.set(true);
  }

  protected openEdit(item: TranslationItem): void {
    this.editingId.set(item.id);
    this.formKey.set(item.key);
    this.formLanguage.set(item.language);
    this.formValue.set(item.value);
    this.showModal.set(true);
  }

  protected closeModal(): void {
    this.showModal.set(false);
  }

  protected async save(): Promise<void> {
    const key = this.formKey().trim();
    const value = this.formValue();
    if (!key) {
      this.error.set(this.language.t('ADMIN.TRANSLATIONS_ERR_KEY'));
      return;
    }
    this.saving.set(true);
    this.error.set('');
    try {
      if (this.editingId()) {
        await this.translationService.update(this.editingId()!, key, this.formLanguage(), value);
      } else {
        await this.translationService.create(key, this.formLanguage(), value);
      }
      this.showModal.set(false);
      await this.load();
    } catch (e) {
      this.error.set(this.extractMessage(e));
    } finally {
      this.saving.set(false);
    }
  }

  protected async remove(item: TranslationItem): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: this.language.t('ADMIN.TRANSLATIONS_DELETE_TITLE'),
      message: this.language.t('ADMIN.TRANSLATIONS_DELETE_MSG', { key: item.key, lang: item.language }),
      danger: true,
    });
    if (!ok) return;
    try {
      await this.translationService.delete(item.id);
      await this.load();
    } catch (e) {
      this.error.set(this.extractMessage(e));
    }
  }

  private extractMessage(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('ADMIN.TRANSLATIONS_ERR_DEFAULT'));
  }
}
