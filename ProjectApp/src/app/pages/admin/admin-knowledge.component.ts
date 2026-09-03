import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { ConfirmService } from '../../core/services/confirm.service';
import { KnowledgeService, KnowledgeEntry } from '../../core/services/knowledge.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-admin-knowledge',
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './admin-knowledge.component.html',
  styleUrl: './admin-knowledge.component.scss',
})
export class AdminKnowledgeComponent implements OnInit {
  private readonly svc = inject(KnowledgeService);
  private readonly confirmService = inject(ConfirmService);
  private readonly language = inject(LanguageService);

  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected readonly entries = signal<KnowledgeEntry[]>([]);
  protected readonly search = signal('');

  /* ---- Form thêm/sửa ---- */
  protected readonly showForm = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly form = {
    keywords: '',
    title: '',
    content: '',
    isActive: true,
  };

  protected readonly filtered = () => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.entries();
    return this.entries().filter(
      (e) =>
        e.title.toLowerCase().includes(q) ||
        e.keywords.toLowerCase().includes(q) ||
        e.content.toLowerCase().includes(q),
    );
  };

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  private async reload(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      this.entries.set(await this.svc.getAll());
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.loading.set(false);
    }
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.form.keywords = '';
    this.form.title = '';
    this.form.content = '';
    this.form.isActive = true;
    this.showForm.set(true);
    this.error.set('');
  }

  protected openEdit(e: KnowledgeEntry): void {
    this.editingId.set(e.id);
    this.form.keywords = e.keywords;
    this.form.title = e.title;
    this.form.content = e.content;
    this.form.isActive = e.isActive;
    this.showForm.set(true);
    this.error.set('');
  }

  protected closeForm(): void {
    this.showForm.set(false);
    this.editingId.set(null);
  }

  protected async save(): Promise<void> {
    this.error.set('');
    this.success.set('');
    if (!this.form.title.trim() || !this.form.content.trim() || !this.form.keywords.trim()) {
      this.error.set(this.language.t('KNOWLEDGE.ERR_REQUIRED'));
      return;
    }

    this.saving.set(true);
    try {
      const payload = {
        keywords: this.form.keywords.trim(),
        title: this.form.title.trim(),
        content: this.form.content.trim(),
        isActive: this.form.isActive,
      };
      if (this.editingId()) {
        await this.svc.update(this.editingId()!, payload);
        this.success.set(this.language.t('KNOWLEDGE.UPDATED'));
      } else {
        await this.svc.create(payload);
        this.success.set(this.language.t('KNOWLEDGE.CREATED'));
      }
      this.closeForm();
      await this.reload();
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.saving.set(false);
    }
  }

  protected async remove(e: KnowledgeEntry): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: this.language.t('KNOWLEDGE.DELETE_TITLE'),
      message: this.language.t('KNOWLEDGE.DELETE_MSG', { title: e.title }),
      confirmText: this.language.t('COMMON.DELETE'),
      danger: true,
    });
    if (!ok) return;
    try {
      await this.svc.remove(e.id);
      this.success.set(this.language.t('KNOWLEDGE.DELETED'));
      await this.reload();
    } catch (err) {
      this.error.set(this.extractError(err));
    }
  }

  protected async toggleActive(e: KnowledgeEntry): Promise<void> {
    try {
      await this.svc.update(e.id, {
        keywords: e.keywords,
        title: e.title,
        content: e.content,
        isActive: !e.isActive,
      });
      await this.reload();
    } catch (err) {
      this.error.set(this.extractError(err));
    }
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('KNOWLEDGE.ERR_DEFAULT'));
  }
}
