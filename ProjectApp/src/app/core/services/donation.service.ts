import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { TransactionDto } from './transaction.service';

/** Tài khoản tiếp nhận của một quỹ (tại một ngân hàng). */
export interface DonationBankAccount {
  bankCode: string;
  bankName: string;
  accountNumber: string;
  currency: string;
}

/** Quỹ quyên góp / ủng hộ. */
export interface DonationFund {
  id: string;
  name: string;
  organization: string;
  description: string;
  accounts: DonationBankAccount[];
}

/**
 * Tầng dữ liệu ủng hộ MTTQ / từ thiện — gọi backend API thật.
 *   - getFunds() → GET  /api/donations/funds
 *   - donate()   → POST /api/donations
 */
@Injectable({ providedIn: 'root' })
export class DonationService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  /** Danh sách quỹ quyên góp / ủng hộ. */
  async getFunds(): Promise<DonationFund[]> {
    return firstValueFrom(
      this.http.get<DonationFund[]>(`${this.apiUrl}/donations/funds`),
    );
  }

  /** Thực hiện giao dịch ủng hộ (miễn phí, lưu lịch sử giao dịch). */
  async donate(params: {
    fromAccountId: string;
    fundId: string;
    bankCode: string;
    amount: number;
    message?: string;
    transactionPassword?: string;
  }): Promise<TransactionDto> {
    return firstValueFrom(
      this.http.post<TransactionDto>(`${this.apiUrl}/donations`, {
        fromAccountId: params.fromAccountId,
        fundId: params.fundId,
        bankCode: params.bankCode,
        amount: params.amount,
        message: params.message ?? null,
        transactionPassword: params.transactionPassword ?? null,
      }),
    );
  }
}
