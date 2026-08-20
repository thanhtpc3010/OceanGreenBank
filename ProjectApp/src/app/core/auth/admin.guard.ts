import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/** Admin Guard — chỉ user có quyền USER.READ (hoặc ADMIN.ALL) mới vào được. */
export const adminGuard: CanActivateFn = (): boolean => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn() && auth.hasPermission('USER.READ')) {
    return true;
  }

  router.navigate(['/dashboard']);
  return false;
};
