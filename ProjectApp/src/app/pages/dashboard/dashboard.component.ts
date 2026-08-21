import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, signal } from '@angular/core';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

interface DonutSegment {
  label: string;
  value: number;
  color: string;
}

interface CashflowItem {
  month: string;
  income: number;
  expense: number;
}

interface RecentTx {
  desc: string;
  amount: number;
  time: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [DecimalPipe, DatePipe, CardModule, ChartModule, TableModule, ButtonModule, TagModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  protected readonly today = new Date();
  protected readonly userName = signal('Nguyễn Văn A');

  // CASA
  protected readonly accountNumber = signal('9999 8888');
  protected readonly balance = signal(15_000_000);

  // AutoEarn
  protected readonly autoEarnActive = signal(true);
  protected readonly interestRate = signal(4.5);
  protected readonly monthlyAccum = signal(42_500);

  // ===== Donut chart: chi tiêu =====
  protected readonly donutSegments: DonutSegment[] = [
    { label: 'Ăn uống', value: 4_500_000, color: '#10b981' },
    { label: 'Mua sắm', value: 2_800_000, color: '#0ea5e9' },
    { label: 'Hóa đơn', value: 2_000_000, color: '#8b5cf6' },
    { label: 'Di chuyển', value: 1_200_000, color: '#f59e0b' },
    { label: 'Khác', value: 900_000, color: '#f43f5e' },
  ];
  protected readonly donutTotal = this.donutSegments.reduce((s, x) => s + x.value, 0);

  protected readonly donutData: any = {
    labels: this.donutSegments.map((s) => s.label),
    datasets: [
      {
        data: this.donutSegments.map((s) => s.value),
        backgroundColor: this.donutSegments.map((s) => s.color),
        hoverBackgroundColor: this.donutSegments.map((s) => s.color),
        borderWidth: 2,
        borderColor: '#ffffff',
      },
    ],
  };

  protected readonly donutOptions: any = {
    cutout: '72%',
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context: { label?: string; parsed?: number }) =>
            `${context.label ?? ''}: ${new Intl.NumberFormat('vi-VN').format(context.parsed ?? 0)} VND`,
        },
      },
    },
  };

  // ===== Bar chart: cashflow 6 tháng =====
  protected readonly cashflow: CashflowItem[] = [
    { month: 'T3', income: 16_000_000, expense: 10_500_000 },
    { month: 'T4', income: 17_500_000, expense: 12_000_000 },
    { month: 'T5', income: 15_000_000, expense: 9_800_000 },
    { month: 'T6', income: 18_200_000, expense: 13_400_000 },
    { month: 'T7', income: 19_000_000, expense: 11_200_000 },
    { month: 'T8', income: 16_800_000, expense: 10_900_000 },
  ];

  protected readonly cashflowData: any = {
    labels: this.cashflow.map((c) => c.month),
    datasets: [
      {
        label: 'Thu',
        data: this.cashflow.map((c) => c.income),
        backgroundColor: '#10b981',
        borderRadius: 4,
      },
      {
        label: 'Chi',
        data: this.cashflow.map((c) => c.expense),
        backgroundColor: '#0ea5e9',
        borderRadius: 4,
      },
    ],
  };

  protected readonly cashflowOptions: any = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { grid: { display: false } },
      y: {
        beginAtZero: true,
        grid: { color: 'rgba(148,163,184,0.15)' },
        ticks: {
          callback: (value: string | number) =>
            new Intl.NumberFormat('vi-VN', { notation: 'compact' }).format(Number(value)),
        },
      },
    },
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context: { dataset?: { label?: string }; parsed?: { y?: number } }) =>
            `${context.dataset?.label ?? ''}: ${new Intl.NumberFormat('vi-VN').format(context.parsed?.y ?? 0)} VND`,
        },
      },
    },
  };

  // ===== Giao dịch gần đây =====
  protected readonly recentTransactions: RecentTx[] = [
    { desc: 'Chuyển tiền nội bộ', amount: -500_000, time: '08:24 hôm nay', color: '#f43f5e' },
    { desc: 'Nhận lương tháng 8', amount: 18_000_000, time: '09:00 hôm nay', color: '#10b981' },
    { desc: 'Thanh toán hóa đơn điện', amount: -350_000, time: 'Hôm qua', color: '#f43f5e' },
    { desc: 'Tích lũy AutoEarn', amount: 42_500, time: 'Hôm qua', color: '#10b981' },
    { desc: 'Mua sắm online', amount: -1_200_000, time: '06/08', color: '#f43f5e' },
  ];

  protected format(n: number): string {
    return new Intl.NumberFormat('vi-VN').format(n);
  }
}
