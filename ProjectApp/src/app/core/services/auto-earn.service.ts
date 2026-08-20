import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Cấu hình AutoEarn (khớp AutoEarnSettingDto backend). */
export interface AutoEarnSetting {
  isActive: boolean;
  annualInterestRate: number;
  runTime: string;
  lastRunAt: string | null;
  nextRunAt: string | null;
}

/** Tài khoản tham gia AutoEarn của user (khớp AutoEarnAccountDto). */
export interface AutoEarnEnrolledAccount {
  accountId: string;
  accountNumber: string;
  principal: number;
  isEnrolled: boolean;
}

/** Bảng tổng hợp AutoEarn cho dashboard (khớp AutoEarnSummaryDto). */
export interface AutoEarnSummary {
  isActive: boolean;
  annualInterestRate: number;
  runTime: string;
  lastRunAt: string | null;
  nextRunAt: string | null;
  totalPrincipal: number;
  monthlyAccum: number;
  enrolledAccounts: AutoEarnEnrolledAccount[];
}

/** Nhật ký sinh lời (khớp AutoEarnLogDto). */
export interface AutoEarnLog {
  id: string;
  accountId: string;
  accountNumber: string;
  runDate: string;
  principal: number;
  interestAmount: number;
  annualRate: number;
  createdDate: string;
}

/** Tài khoản + trạng thái AutoEarn cho admin (khớp AutoEarnAccountAdminDto). */
export interface AutoEarnAccountAdmin {
  accountId: string;
  accountNumber: string;
  ownerName: string;
  balance: number;
  isEnrolled: boolean;
  principal: number;
}

/**
 * Tầng dữ liệu AutoEarn — gọi backend API thật.
 *   - getSettings()            → GET  /api/auto-earn/settings
 *   - updateSettings(patch)    → PUT  /api/auto-earn/settings   (admin)
 *   - getSummary(userId)       → GET  /api/auto-earn/summary/{userId}
 *   - runNow()                 → POST /api/auto-earn/run-now    (admin)
 *   - getLogs()                → GET  /api/auto-earn/logs       (admin)
 *   - getAccounts()            → GET  /api/auto-earn/accounts   (admin)
 *   - updateAccountEnrollment()→ PUT  /api/auto-earn/accounts/{id} (admin)
 */
@Injectable({ providedIn: 'root' })
export class AutoEarnService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  getSettings(): Promise<AutoEarnSetting> {
    return firstValueFrom(this.http.get<AutoEarnSetting>(`${this.apiUrl}/auto-earn/settings`));
  }

  updateSettings(patch: { isActive?: boolean; annualInterestRate?: number; runTime?: string }): Promise<AutoEarnSetting> {
    return firstValueFrom(
      this.http.put<AutoEarnSetting>(`${this.apiUrl}/auto-earn/settings`, {
        isActive: patch.isActive ?? null,
        annualInterestRate: patch.annualInterestRate ?? null,
        runTime: patch.runTime ?? null,
      }),
    );
  }

  getSummary(userId: string): Promise<AutoEarnSummary> {
    return firstValueFrom(this.http.get<AutoEarnSummary>(`${this.apiUrl}/auto-earn/summary/${userId}`));
  }

  runNow(): Promise<AutoEarnSetting> {
    return firstValueFrom(this.http.post<AutoEarnSetting>(`${this.apiUrl}/auto-earn/run-now`, {}));
  }

  getLogs(): Promise<AutoEarnLog[]> {
    return firstValueFrom(this.http.get<AutoEarnLog[]>(`${this.apiUrl}/auto-earn/logs`));
  }

  getAccounts(): Promise<AutoEarnAccountAdmin[]> {
    return firstValueFrom(this.http.get<AutoEarnAccountAdmin[]>(`${this.apiUrl}/auto-earn/accounts`));
  }

  updateAccountEnrollment(accountId: string, isEnrolled: boolean, principal: number): Promise<AutoEarnAccountAdmin> {
    return firstValueFrom(
      this.http.put<AutoEarnAccountAdmin>(`${this.apiUrl}/auto-earn/accounts/${accountId}`, {
        isEnrolled,
        principal,
      }),
    );
  }
}
