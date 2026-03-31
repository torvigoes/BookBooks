import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { API_BASE_URL } from './api.tokens';

export const apiUrlInterceptor: HttpInterceptorFn = (req, next) => {
  if (/^https?:\/\//i.test(req.url)) {
    return next(req);
  }

  const baseUrl = inject(API_BASE_URL);
  const normalizedBaseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
  const normalizedPath = req.url.startsWith('/') ? req.url : `/${req.url}`;

  return next(req.clone({ url: `${normalizedBaseUrl}${normalizedPath}` }));
};
