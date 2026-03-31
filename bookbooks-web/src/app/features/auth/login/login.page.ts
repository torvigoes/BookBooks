import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../../core/auth/auth-api.service';
import { AuthSessionService } from '../../../core/auth/auth-session.service';
import { HttpErrorService } from '../../../core/api/http-error.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-card">
      <h1>Login</h1>
      <p class="hint">Use your BookBooks credentials.</p>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <label for="email">Email</label>
        <input id="email" type="email" formControlName="email" />

        <label for="password">Password</label>
        <input id="password" type="password" formControlName="password" />

        @if (errorMessage(); as message) {
          <p class="error">{{ message }}</p>
        }

        <button type="submit" [disabled]="isSubmitting() || form.invalid">
          {{ isSubmitting() ? 'Signing in...' : 'Sign in' }}
        </button>
      </form>

      <p class="footer-link">
        No account yet?
        <a routerLink="/register">Create one</a>
      </p>
    </section>
  `,
  styles: `
    .auth-card {
      background: #fff;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      margin: 0 auto;
      max-width: 420px;
      padding: 1rem;
    }

    .hint {
      color: #475569;
      margin-top: 0;
    }

    form {
      display: grid;
      gap: 0.6rem;
    }

    label {
      color: #334155;
      font-size: 0.9rem;
      font-weight: 600;
    }

    input {
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      padding: 0.55rem 0.7rem;
    }

    button {
      background: #1d4ed8;
      border: 0;
      border-radius: 8px;
      color: #fff;
      cursor: pointer;
      margin-top: 0.4rem;
      padding: 0.6rem 0.8rem;
    }

    button:disabled {
      cursor: not-allowed;
      opacity: 0.7;
    }

    .error {
      color: #b91c1c;
      margin: 0.3rem 0;
    }

    .footer-link {
      color: #475569;
      margin: 1rem 0 0;
    }
  `
})
export class LoginPage {
  private readonly authApi = inject(AuthApiService);
  private readonly authSession = inject(AuthSessionService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly httpErrorService = inject(HttpErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly isSubmitting = signal(false);
  readonly localErrorMessage = signal<string | null>(null);
  readonly errorMessage = computed(() => this.localErrorMessage() ?? this.httpErrorService.message());

  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  constructor() {
    if (this.authSession.isAuthenticated()) {
      this.router.navigateByUrl('/');
    }
  }

  submit(): void {
    if (this.form.invalid || this.isSubmitting()) {
      return;
    }

    this.localErrorMessage.set(null);
    this.httpErrorService.clear();
    this.isSubmitting.set(true);

    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    const request = this.form.getRawValue();

    this.authApi
      .login(request)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (response) => {
          this.authSession.signIn({
            userId: response.id,
            username: response.username,
            email: response.email,
            token: response.token
          });
          this.router.navigateByUrl(returnUrl);
        },
        error: () => {
          if (!this.httpErrorService.message()) {
            this.localErrorMessage.set('Failed to login.');
          }
        }
      });
  }
}
