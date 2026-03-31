import { Injectable, computed, signal } from '@angular/core';
import { AuthSession } from '../models/auth-session.model';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly _session = signal<AuthSession | null>(null);

  readonly session = computed(() => this._session());
  readonly isAuthenticated = computed(() => this._session() !== null);
  readonly token = computed(() => this._session()?.token ?? null);

  signIn(session: AuthSession): void {
    this._session.set(session);
  }

  signOut(): void {
    this._session.set(null);
  }
}
