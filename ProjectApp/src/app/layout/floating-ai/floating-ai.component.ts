import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ChatService, ChatHistoryItem } from '../../core/services/chat.service';

interface ChatMessage {
  from: 'bot' | 'user';
  text: string;
}

@Component({
  selector: 'app-floating-ai',
  imports: [FormsModule],
  templateUrl: './floating-ai.component.html',
  styleUrl: './floating-ai.component.scss',
})
export class FloatingAiComponent implements OnInit {
  private readonly chat = inject(ChatService);

  protected readonly open = signal(false);
  protected readonly enabled = signal(true);
  protected readonly sending = signal(false);
  protected readonly messages = signal<ChatMessage[]>([
    {
      from: 'bot',
      text: 'Xin chào! Mình là trợ lý SmartBank 🤖 Mình có thể hướng dẫn các chức năng của app hoặc trả lời về tài khoản của bạn.',
    },
  ]);
  protected readonly input = signal('');

  /** Lịch sử gửi kèm để bot nhớ ngữ cảnh (tối đa 10 tin). */
  private history: ChatHistoryItem[] = [];

  async ngOnInit(): Promise<void> {
    try {
      const st = await this.chat.getStatus();
      this.enabled.set(st.enabled);
    } catch {
      this.enabled.set(false);
    }
  }

  protected toggle(): void {
    this.open.update((v) => !v);
  }

  protected async send(): Promise<void> {
    const text = this.input().trim();
    if (!text || this.sending()) return;

    this.messages.update((m) => [...m, { from: 'user', text }]);
    this.input.set('');
    this.sending.set(true);

    try {
      const res = await this.chat.sendMessage(text, this.history);
      this.history = [
        ...this.history.slice(-9),
        { role: 'user', content: text },
        { role: 'model', content: res.reply },
      ];
      this.messages.update((m) => [...m, { from: 'bot', text: res.reply }]);
    } catch (e) {
      const body = (e as { error?: { message?: string } })?.error;
      this.messages.update((m) => [
        ...m,
        {
          from: 'bot',
          text: body?.message ?? 'Có lỗi khi kết nối AI. Vui lòng thử lại sau.',
        },
      ]);
    } finally {
      this.sending.set(false);
    }
  }
}
