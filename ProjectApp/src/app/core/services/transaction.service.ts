import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Enum transaction type (khớp backend). */
export enum TxType {
  InternalTransfer = 1,
  InterbankTransfer = 2,
}

/** Enum transaction status (khớp backend). */
export enum TxStatus {
  Pending = 1,
  Success = 2,
  Failed = 3,
}

/** Danh mục thu chi do user chọn (khớp backend TransactionCategory). */
export enum TxCategory {
  Other = 0,
  Food = 1,
  Shopping = 2,
  Bills = 3,
  Transport = 4,
  Entertainment = 5,
  Health = 6,
  Education = 7,
  Savings = 8,
  Transfer = 9,
}

export const CATEGORY_LABELS: Record<TxCategory, string> = {
  [TxCategory.Other]: 'Khác',
  [TxCategory.Food]: 'Ăn uống',
  [TxCategory.Shopping]: 'Mua sắm',
  [TxCategory.Bills]: 'Hóa đơn',
  [TxCategory.Transport]: 'Di chuyển',
  [TxCategory.Entertainment]: 'Giải trí',
  [TxCategory.Health]: 'Y tế',
  [TxCategory.Education]: 'Giáo dục',
  [TxCategory.Savings]: 'Tiết kiệm',
  [TxCategory.Transfer]: 'Chuyển khoản',
};

/** Danh sách danh mục cho dropdown. */
export const CATEGORY_OPTIONS: { value: TxCategory; label: string }[] = (
  Object.keys(CATEGORY_LABELS) as unknown as TxCategory[]
).map((v) => ({ value: Number(v) as TxCategory, label: CATEGORY_LABELS[v] }));

/** Giao dịch (khớp TransactionDto backend). */
export interface TransactionDto {
  id: string;
  transactionCode: string;
  fromAccountId: string;
  toAccountId: string | null;
  receiverAccount: string | null;
  receiverName: string | null;
  receiverBankCode: string | null;
  amount: number;
  fee: number;
  description: string | null;
  status: TxStatus;
  type: TxType;
  category: TxCategory;
  createdDate: string;
}

/** Thông tin người nhận sau khi tra cứu. */
export interface ReceiverInfo {
  accountId: string;
  accountNumber: string;
  ownerName: string;
}

/** Chi tiết tổng chi theo danh mục (từ API PFM). */
export interface CategorySummary {
  category: TxCategory;
  categoryName: string;
  total: number;
  count: number;
}

/** Tổng hợp thu chi PFM (từ API PFM). */
export interface PfmSummaryDto {
  totalIncome: number;
  totalExpense: number;
  net: number;
  expenseByCategory: CategorySummary[];
}

/**
 * Tầng dữ liệu giao dịch — gọi backend API thật.
 *   - transfer()             → POST /api/transactions
 *   - getTransactions(id)    → GET  /api/accounts/{id}/transactions
 *   - findAccountByNumber()  → quét users+accounts để tìm tài khoản nhận
 */
@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  /** Tạo giao dịch chuyển tiền — POST /api/transactions. */
  async transfer(params: {
    fromAccountId: string;
    type: TxType;
    amount: number;
    description?: string;
    category?: TxCategory;
    toAccountId?: string;
    receiverAccount?: string;
    receiverName?: string;
    receiverBankCode?: string;
    isEarlyWithdrawal?: boolean;
  }): Promise<TransactionDto> {
    return firstValueFrom(
      this.http.post<TransactionDto>(`${this.apiUrl}/transactions`, {
        fromAccountId: params.fromAccountId,
        type: params.type,
        amount: params.amount,
        description: params.description ?? null,
        category: params.category ?? TxCategory.Other,
        toAccountId: params.toAccountId ?? null,
        receiverAccount: params.receiverAccount ?? null,
        receiverName: params.receiverName ?? null,
        receiverBankCode: params.receiverBankCode ?? null,
        isEarlyWithdrawal: params.isEarlyWithdrawal ?? false,
      }),
    );
  }

  /** Lịch sử giao dịch của tài khoản — GET /api/accounts/{id}/transactions. */
  async getTransactions(accountId: string): Promise<TransactionDto[]> {
    return firstValueFrom(
      this.http.get<TransactionDto[]>(`${this.apiUrl}/accounts/${accountId}/transactions`),
    );
  }

  /* ============ PFM (BOT tổng hợp thu chi) ============ */

  /** Tổng hợp thu/chi theo danh mục — GET /api/pfm/summary/{userId}. */
  async getPfmSummary(userId: string): Promise<PfmSummaryDto> {
    return firstValueFrom(
      this.http.get<PfmSummaryDto>(`${this.apiUrl}/pfm/summary/${userId}`),
    );
  }

  /**
   * Tra cứu tài khoản nhận theo số tài khoản (chuyển nội bộ).
   * Quét toàn bộ users + accounts để tìm khớp số tài khoản → trả về id + tên chủ tài khoản.
   */
  async findAccountByNumber(accountNumber: string): Promise<ReceiverInfo | null> {
    const compact = accountNumber.replace(/\s/g, '');
    const users = await firstValueFrom(
      this.http.get<{ id: string; fullName: string; email: string }[]>(`${this.apiUrl}/users`),
    );
    for (const user of users) {
      const accounts = await firstValueFrom(
        this.http.get<{ id: string; accountNumber: string }[]>(
          `${this.apiUrl}/accounts/by-user/${user.id}`,
        ),
      );
      const match = accounts.find((a) => a.accountNumber === compact);
      if (match) {
        return { accountId: match.id, accountNumber: match.accountNumber, ownerName: user.fullName };
      }
    }
    return null;
  }
}
