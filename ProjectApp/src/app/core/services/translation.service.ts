import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Bản dịch (khớp TranslationDto backend). */
export interface TranslationItem {
  id: string;
  key: string;
  language: string;
  value: string;
}

/**
 * Tầng dữ liệu quản lý bản dịch (admin) — gọi backend API.
 *   - getAll()  → GET    /api/translations
 *   - create()  → POST   /api/translations
 *   - update()  → PUT    /api/translations/{id}
 *   - delete()  → DELETE /api/translations/{id}
 */
@Injectable({ providedIn: 'root' })
export class TranslationService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  async getAll(): Promise<TranslationItem[]> {
    return firstValueFrom(this.http.get<TranslationItem[]>(`${this.apiUrl}/translations`));
  }

  async create(key: string, language: string, value: string): Promise<TranslationItem> {
    return firstValueFrom(
      this.http.post<TranslationItem>(`${this.apiUrl}/translations`, { key, language, value }),
    );
  }

  async update(id: string, key: string, language: string, value: string): Promise<TranslationItem> {
    return firstValueFrom(
      this.http.put<TranslationItem>(`${this.apiUrl}/translations/${id}`, { key, language, value }),
    );
  }

  async delete(id: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${this.apiUrl}/translations/${id}`));
  }
}
