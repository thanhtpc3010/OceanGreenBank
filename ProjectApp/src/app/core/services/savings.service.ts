import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

/** Kế hoạch tiết kiệm định kỳ (khớp SavingsPlanDto backend). */
export interface SavingsPlanDto {
  id: string;
  userId: string;
  sourceAccountId: string;
  sourceAccountNumber: string;
  targetAccountId: string;
  targetAccountNumber: string;
  amount: number;
  cycle: string; // DAILY / WEEKLY / MONTHLY
  startDate: string;
  nextDepositDate: string | null;
  isActive: boolean;
  totalDeposits: number;
  totalSaved: number;
  createdDate: string;
}

export const CYCLE_OPTIONS = [
  { value: 'DAILY', label: 'Hằng ngày' },
  { value: 'WEEKLY', label: 'Hằng tuần' },
  { value: 'MONTHLY', label: 'Hằng tháng' },
];

/**
 * Dịch vụ sổ tiết kiệm theo chu kỳ — gọi backend API.
 *   - createPlan() → POST /api/savings-plans
 *   - getPlans()   → GET  /api/savings-plans/by-user/{userId}
 *   - deposit()    → POST /api/savings-plans/{id}/deposit
 *   - cancelPlan() → DELETE /api/savings-plans/{id}
 */
@Injectable({ providedIn: 'root' })
export class SavingsService {
  private readonly apiUrl = 'http://localhost:5081/api';

  constructor(private readonly http: HttpClient) {}

  async createPlan(data: {
    userId: string;
    sourceAccountId: string;
    targetAccountId: string;
    amount: number;
    cycle: string;
    startDate: string;
  }): Promise<SavingsPlanDto> {
    return firstValueFrom(
      this.http.post<SavingsPlanDto>(`${this.apiUrl}/savings-plans`, data),
    );
  }

  async getPlans(userId: string): Promise<SavingsPlanDto[]> {
    return firstValueFrom(
      this.http.get<SavingsPlanDto[]>(`${this.apiUrl}/savings-plans/by-user/${userId}`),
    );
  }

  async deposit(planId: string): Promise<SavingsPlanDto> {
    return firstValueFrom(
      this.http.post<SavingsPlanDto>(`${this.apiUrl}/savings-plans/${planId}/deposit`, {}),
    );
  }

  async cancelPlan(planId: string): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.apiUrl}/savings-plans/${planId}`));
  }

  cycleLabel(cycle: string): string {
    return CYCLE_OPTIONS.find((c) => c.value === cycle)?.label ?? cycle;
  }
}
