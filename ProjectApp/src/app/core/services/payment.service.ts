import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Nhà cung cấp ví thanh toán (khớp backend PaymentProvider). */
export enum PaymentProvider {
  Momo = 1,
  ZaloPay = 2,
  Cash = 3,
}

/** Trạng thái đơn nạp tiền (khớp backend PaymentStatus). */
export enum PaymentStatus {
  Pending = 1,
  Success = 2,
  Failed = 3,
}

/** Giao dịch nạp tiền qua ví (khớp PaymentDto backend). */
export interface PaymentDto {
  id: string;
  userId: string;
  provider: number;
  providerName: string;
  accountId: string;
  accountNumber: string;
  amount: number;
  orderCode: string | null;
  status: number;
  statusName: string;
  description: string | null;
  completedDate: string | null;
  createdDate: string;
  mockPaymentUrl: string;
}

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5081/api';

  /** Tạo đơn nạp tiền qua ví (mock) → trả về mockPaymentUrl để redirect. */
  createPayment(payload: {
    provider: number;
    accountId: string;
    amount: number;
    description?: string;
  }): Promise<PaymentDto> {
    return firstValueFrom(this.http.post<PaymentDto>(`${this.apiUrl}/payments`, payload));
  }

  /** Ví xác nhận thanh toán (mock IPN) → credit tiền vào tài khoản. */
  confirmPayment(id: string): Promise<PaymentDto> {
    return firstValueFrom(this.http.post<PaymentDto>(`${this.apiUrl}/payments/${id}/confirm`, {}));
  }

  /** Hủy thanh toán. */
  cancelPayment(id: string): Promise<PaymentDto> {
    return firstValueFrom(this.http.post<PaymentDto>(`${this.apiUrl}/payments/${id}/cancel`, {}));
  }

  /** Lấy chi tiết đơn thanh toán theo id. */
  getPayment(id: string): Promise<PaymentDto> {
    return firstValueFrom(this.http.get<PaymentDto>(`${this.apiUrl}/payments/${id}`));
  }

  /** Lịch sử nạp tiền của user. */
  getPayments(userId: string): Promise<PaymentDto[]> {
    return firstValueFrom(this.http.get<PaymentDto[]>(`${this.apiUrl}/payments/by-user/${userId}`));
  }
}
