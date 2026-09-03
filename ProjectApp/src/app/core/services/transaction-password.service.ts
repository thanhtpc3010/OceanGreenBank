import { Injectable, signal } from '@angular/core';

export interface TransactionPasswordOptions {
  /** Tiêu đề popup. */
  title?: string;
  /** Ngữ cảnh hiển thị (vd: "Chuyển 100.000 VND đến..."). */
  message?: string;
  /** Nhãn nút xác nhận (mặc định "Xác nhận"). */
  confirmText?: string;
}

interface TransactionPasswordState {
  options: TransactionPasswordOptions;
  resolve: (password: string | null) => void;
}

/**
 * Dịch vụ popup nhập mật khẩu giao dịch (cấp 2) — dùng chung cho mọi màn hình
 * cần xác thực giao dịch (chuyển tiền, ủng hộ...).
 *
 * Cách dùng:
 *   const password = await txPasswordService.ask({
 *     message: `Chuyển ${amount} VND đến ${receiver}`,
 *   });
 *   if (password === null) return; // user hủy
 *   // ...gọi API với `password`...
 */
@Injectable({ providedIn: 'root' })
export class TransactionPasswordService {
  /** Trạng thái popup hiện tại (null = đang đóng). */
  readonly state = signal<TransactionPasswordState | null>(null);

  /** Mở popup nhập mật khẩu — trả về Promise<string | null> (null = user hủy). */
  ask(options: TransactionPasswordOptions = {}): Promise<string | null> {
    return new Promise((resolve) => {
      this.state.set({ options, resolve });
    });
  }

  /** Đóng popup với mật khẩu đã nhập (hoặc null khi hủy). */
  resolve(password: string | null): void {
    const current = this.state();
    if (current) {
      current.resolve(password);
    }
    this.state.set(null);
  }
}
