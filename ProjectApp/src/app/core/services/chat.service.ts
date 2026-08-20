import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Một tin trong lịch sử hội thoại gửi lên backend. */
export interface ChatHistoryItem {
  role: 'user' | 'model';
  content: string;
}

/** Phản hồi từ backend (ChatResponse). */
export interface ChatResponse {
  reply: string;
  enabled: boolean;
}

/**
 * Tầng dữ liệu chat AI — gọi backend proxy (giữ API key ở server).
 *   - getStatus()  → GET  /api/chat/status
 *   - sendMessage()→ POST /api/chat
 */
@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  getStatus(): Promise<{ enabled: boolean }> {
    return firstValueFrom(this.http.get<{ enabled: boolean }>(`${this.apiUrl}/chat/status`));
  }

  sendMessage(message: string, history: ChatHistoryItem[]): Promise<ChatResponse> {
    return firstValueFrom(
      this.http.post<ChatResponse>(`${this.apiUrl}/chat`, {
        message,
        history: history.slice(-10),
      }),
    );
  }
}
