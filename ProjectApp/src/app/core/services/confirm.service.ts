import { Injectable, signal } from '@angular/core';

export interface ConfirmOptions {
  /** Tiêu đề popup. */
  title?: string;
  /** Nội dung thông báo. */
  message: string;
  /** Nhãn nút xác nhận (mặc định "Xác nhận"). */
  confirmText?: string;
  /** Nhãn nút hủy (mặc định "Hủy"). */
  cancelText?: string;
  /** Dùng màu đỏ cho thao tác nguy hiểm (xóa...). */
  danger?: boolean;
}

interface ConfirmState {
  options: ConfirmOptions;
  resolve: (value: boolean) => void;
}

/**
 * Dịch vụ popup xác nhận chung (reusable).
 *
 * Cách dùng:
 *   const ok = await confirmService.confirm({
 *     title: 'Xóa tài khoản',
 *     message: 'Bạn chắc chắn muốn xóa?',
 *     danger: true,
 *   });
 *   if (ok) { ...thực hiện thao tác thay đổi data... }
 */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  /** Trạng thái popup hiện tại (null = đang đóng). */
  readonly state = signal<ConfirmState | null>(null);

  /** Mở popup xác nhận — trả về Promise<boolean> (true = user đồng ý). */
  confirm(options: ConfirmOptions): Promise<boolean> {
    return new Promise((resolve) => {
      this.state.set({ options, resolve });
    });
  }

  /** Đóng popup với kết quả. */
  resolve(value: boolean): void {
    const current = this.state();
    if (current) {
      current.resolve(value);
    }
    this.state.set(null);
  }
}
