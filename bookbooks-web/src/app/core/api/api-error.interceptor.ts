import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { HttpErrorService } from './http-error.service';

export const apiErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(HttpErrorService);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        const payload = error.error as { error?: string } | null;
        const apiMessage = payload?.error?.trim();
        const message = apiMessage || `Request failed with status ${error.status}.`;
        errorService.setMessage(message);
      } else {
        errorService.setMessage('Unexpected request error.');
      }

      return throwError(() => error);
    })
  );
};
