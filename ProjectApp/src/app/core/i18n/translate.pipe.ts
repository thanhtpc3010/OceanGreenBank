import { Pipe, PipeTransform } from '@angular/core';

import { LanguageService } from './language.service';

/**
 * Pipe dịch text — `{{ 'KEY' | translate }}`.
 * Pipe impure để tự cập nhật khi đổi ngôn ngữ.
 */
@Pipe({ name: 'translate', standalone: true, pure: false })
export class TranslatePipe implements PipeTransform {
  constructor(private readonly language: LanguageService) {}

  transform(key: string, params?: Record<string, string | number>): string {
    return this.language.t(key, params);
  }
}
