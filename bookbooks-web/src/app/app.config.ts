import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { environment } from '../environments/environment';
import { apiErrorInterceptor } from './core/api/api-error.interceptor';
import { API_BASE_URL } from './core/api/api.tokens';
import { apiUrlInterceptor } from './core/api/api-url.interceptor';
import { authInterceptor } from './core/auth/auth.interceptor';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl },
    provideHttpClient(
      withInterceptors([apiUrlInterceptor, authInterceptor, apiErrorInterceptor])
    )
  ]
};
