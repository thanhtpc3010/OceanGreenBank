import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ScrollPanelModule } from 'primeng/scrollpanel';

interface ChatMessage {
  from: 'bot' | 'user';
  text: string;
}

@Component({
  selector: 'app-floating-ai',
  imports: [FormsModule, ButtonModule, InputTextModule, ScrollPanelModule],
  templateUrl: './floating-ai.component.html',
  styleUrl: './floating-ai.component.scss',
})
export class FloatingAiComponent {
  protected readonly open = signal(false);
  protected readonly messages = signal<ChatMessage[]>([
    { from: 'bot', text: 'Xin chào! Mình là PFM AI Bot 🎯 Hỏi mình về thu chi, tiết kiệm hoặc lãi suất nhé.' },
  ]);
  protected readonly input = signal('');

  protected toggle(): void {
    this.open.update((v) => !v);
  }

  protected send(): void {
    const text = this.input().trim();
    if (!text) return;

    this.messages.update((m) => [...m, { from: 'user', text }]);
    this.input.set('');

    // Mock phản hồi từ AI
    setTimeout(() => {
      this.messages.update((m) => [
        ...m,
        {
          from: 'bot',
          text: 'Cảm ơn bạn! Hiện mình đang là bản demo. Tính năng AI thật sẽ kết nối với backend Gemini sắp tới. 🚀',
        },
      ]);
    }, 600);
  }
}
