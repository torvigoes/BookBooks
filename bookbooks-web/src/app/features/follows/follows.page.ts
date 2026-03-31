import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { FollowsApiService } from '../../core/api/follows-api.service';
import { HttpErrorService } from '../../core/api/http-error.service';
import { FollowedUser } from '../../core/models/followed-user.model';

@Component({
  selector: 'app-follows-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <section class="grid">
      <article class="card">
        <h1>Following</h1>
        @if (isLoading()) {
          <p class="muted">Loading follows...</p>
        } @else if (following().length === 0) {
          <p class="muted">You are not following anyone yet.</p>
        } @else {
          <ul class="follow-items">
            @for (item of following(); track item.userId) {
              <li>
                <div>
                  <strong>{{ item.username || item.userId }}</strong>
                  <p>User ID: {{ item.userId }}</p>
                  <small>Following since {{ formatDate(item.followedAt) }}</small>
                </div>
                <button type="button" class="danger" [disabled]="isSubmitting()" (click)="unfollow(item.userId)">
                  Unfollow
                </button>
              </li>
            }
          </ul>
        }
      </article>

      <article class="card">
        <h2>Follow by user ID</h2>
        <form [formGroup]="form" (ngSubmit)="follow()">
          <input type="text" formControlName="followedUserId" placeholder="User ID" />
          <button type="submit" [disabled]="isSubmitting() || form.invalid">
            {{ isSubmitting() ? 'Following...' : 'Follow' }}
          </button>
        </form>
      </article>
    </section>
  `,
  styles: `
    .grid {
      display: grid;
      gap: 1rem;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    }

    .card {
      background: #fff;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      padding: 1rem;
    }

    .muted {
      color: #64748b;
    }

    .follow-items {
      list-style: none;
      margin: 0.8rem 0 0;
      padding: 0;
    }

    .follow-items li {
      align-items: center;
      border-top: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      padding: 0.7rem 0;
    }

    .follow-items p {
      color: #475569;
      margin: 0.15rem 0;
    }

    .follow-items small {
      color: #64748b;
    }

    form {
      display: grid;
      gap: 0.55rem;
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
      padding: 0.55rem 0.7rem;
      width: fit-content;
    }

    .danger {
      background: #b91c1c;
    }

    button:disabled {
      cursor: not-allowed;
      opacity: 0.7;
    }
  `
})
export class FollowsPage {
  private readonly followsApi = inject(FollowsApiService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly httpErrorService = inject(HttpErrorService);

  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);
  readonly following = signal<FollowedUser[]>([]);

  readonly form = this.formBuilder.nonNullable.group({
    followedUserId: ['', [Validators.required]]
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.httpErrorService.clear();
    this.isLoading.set(true);

    this.followsApi
      .getMine()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (items) => this.following.set(items)
      });
  }

  follow(): void {
    if (this.form.invalid || this.isSubmitting()) {
      return;
    }

    this.httpErrorService.clear();
    const followedUserId = this.form.getRawValue().followedUserId;

    this.isSubmitting.set(true);
    this.followsApi
      .follow(followedUserId)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.form.patchValue({ followedUserId: '' });
          this.load();
        }
      });
  }

  unfollow(followedUserId: string): void {
    if (this.isSubmitting()) {
      return;
    }

    this.httpErrorService.clear();
    this.isSubmitting.set(true);

    this.followsApi
      .unfollow(followedUserId)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => this.load()
      });
  }

  formatDate(isoValue: string): string {
    return new Date(isoValue).toLocaleString();
  }
}
