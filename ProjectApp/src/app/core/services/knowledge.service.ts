import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Một mục kiến thức (khớp KnowledgeEntryDto backend). */
export interface KnowledgeEntry {
  id: string;
  keywords: string;
  title: string;
  content: string;
  isActive: boolean;
  createdDate: string;
}

/**
 * Tầng dữ liệu quản lý kho kiến thức chat bot (admin).
 *   - getAll()      → GET  /api/knowledge
 *   - create(data)  → POST /api/knowledge
 *   - update(id,..) → PUT  /api/knowledge/{id}
 *   - remove(id)    → DELETE /api/knowledge/{id}
 */
@Injectable({ providedIn: 'root' })
export class KnowledgeService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  getAll(): Promise<KnowledgeEntry[]> {
    return firstValueFrom(this.http.get<KnowledgeEntry[]>(`${this.apiUrl}/knowledge`));
  }

  create(data: { keywords: string; title: string; content: string; isActive: boolean }): Promise<KnowledgeEntry> {
    return firstValueFrom(this.http.post<KnowledgeEntry>(`${this.apiUrl}/knowledge`, data));
  }

  update(
    id: string,
    data: { keywords: string; title: string; content: string; isActive: boolean },
  ): Promise<KnowledgeEntry> {
    return firstValueFrom(this.http.put<KnowledgeEntry>(`${this.apiUrl}/knowledge/${id}`, data));
  }

  remove(id: string): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.apiUrl}/knowledge/${id}`));
  }
}
