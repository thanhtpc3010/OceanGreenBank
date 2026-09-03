import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export type AppLanguage = 'vi' | 'en';

export const APP_LANGUAGES: { code: AppLanguage; label: string; flag: string }[] = [
  { code: 'vi', label: 'Tiếng Việt', flag: '🇻🇳' },
  { code: 'en', label: 'English', flag: '🇬🇧' },
];

/**
 * Dịch vụ ngôn ngữ — bản dịch được lưu trong DATABASE (bảng `Translations`)
 * và tải qua API `GET /api/translations/{lang}`. KHÔNG phụ thuộc AI.
 */
@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5081/api';
  private readonly STORAGE_KEY = 'smartbank.lang';

  /** Ngôn ngữ hiện tại. */
  readonly current = signal<AppLanguage>('vi');

  /** Danh sách ngôn ngữ hỗ trợ. */
  readonly languages = APP_LANGUAGES;

  private readonly translations = signal<Record<string, string>>({});

  /** Khởi tạo: đọc ngôn ngữ đã lưu rồi tải bản dịch từ DB. */
  async init(): Promise<void> {
    const saved = localStorage.getItem(this.STORAGE_KEY) as AppLanguage | null;
    await this.setLanguage(saved === 'en' ? 'en' : 'vi');
  }

  /** Đổi ngôn ngữ + tải bản dịch tương ứng từ DB. */
  async setLanguage(lang: AppLanguage): Promise<void> {
    const data = await firstValueFrom(
      this.http.get<Record<string, string>>(`${this.apiUrl}/translations/${lang}`),
    );
    this.translations.set(data);
    this.current.set(lang);
    localStorage.setItem(this.STORAGE_KEY, lang);
    document.documentElement.lang = lang;
  }

  /** Lấy bản dịch theo key, hỗ trợ tham số `{{key}}`. */
  t(key: string, params?: Record<string, string | number>): string {
    let text = this.translations()[key] ?? key;
    if (params) {
      for (const [k, v] of Object.entries(params)) {
        text = text.replaceAll(`{{${k}}}`, String(v));
      }
    }
    return text;
  }
}
