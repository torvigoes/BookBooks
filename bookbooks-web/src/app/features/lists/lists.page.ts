import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ListsApiService } from '../../core/api/lists-api.service';
import { HttpErrorService } from '../../core/api/http-error.service';
import { BookList } from '../../core/models/book-list.model';
import { ListVisibility } from '../../core/models/list-visibility.enum';

@Component({
  selector: 'app-lists-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <section class="grid">
      <article class="card">
        <h1>My lists</h1>
        @if (isLoading()) {
          <p class="muted">Loading lists...</p>
        } @else if (lists().length === 0) {
          <p class="muted">No lists created yet.</p>
        } @else {
          <ul class="list-items">
            @for (list of lists(); track list.id) {
              <li>
                <div>
                  <strong>{{ list.name }}</strong>
                  <p>{{ list.description || 'No description' }}</p>
                  <small>{{ visibilityLabel(list.visibility) }} - {{ list.itemCount }} books</small>
                </div>
                <a [routerLink]="['/lists', list.id]">Details</a>
              </li>
            }
          </ul>
        }
      </article>

      <article class="card">
        <h2>Create list</h2>
        <form [formGroup]="form" (ngSubmit)="create()">
          <input type="text" formControlName="name" placeholder="Name" />
          <textarea rows="3" formControlName="description" placeholder="Description"></textarea>
          <select formControlName="visibility">
            <option [ngValue]="listVisibility.Public">Public</option>
            <option [ngValue]="listVisibility.Private">Private</option>
            <option [ngValue]="listVisibility.FriendsOnly">Friends only</option>
          </select>
          <button type="submit" [disabled]="isSubmitting() || form.invalid">
            {{ isSubmitting() ? 'Creating...' : 'Create list' }}
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

    .list-items {
      list-style: none;
      margin: 0.8rem 0 0;
      padding: 0;
    }

    .list-items li {
      align-items: center;
      border-top: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      padding: 0.7rem 0;
    }

    .list-items p {
      color: #475569;
      margin: 0.15rem 0;
    }

    form {
      display: grid;
      gap: 0.55rem;
    }

    input,
    textarea,
    select {
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
    }

    button:disabled {
      cursor: not-allowed;
      opacity: 0.7;
    }
  `
})
export class ListsPage {
  private readonly listsApi = inject(ListsApiService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly httpErrorService = inject(HttpErrorService);
  private readonly router = inject(Router);

  readonly listVisibility = ListVisibility;
  readonly isLoading = signal(false);
  readonly isSubmitting = signal(false);
  readonly lists = signal<BookList[]>([]);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(120)]],
    description: [''],
    visibility: [ListVisibility.Public, [Validators.required]]
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.httpErrorService.clear();
    this.isLoading.set(true);

    this.listsApi
      .getMine()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (items) => this.lists.set(items),
        error: () => {
          if (this.httpErrorService.message()?.includes('401')) {
            this.router.navigate(['/login']);
          }
        }
      });
  }

  create(): void {
    if (this.form.invalid || this.isSubmitting()) {
      return;
    }

    this.httpErrorService.clear();
    const raw = this.form.getRawValue();

    this.isSubmitting.set(true);
    this.listsApi
      .create({
        name: raw.name,
        description: raw.description.trim() ? raw.description.trim() : null,
        visibility: raw.visibility
      })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (created) => {
          this.form.patchValue({
            name: '',
            description: '',
            visibility: ListVisibility.Public
          });
          this.lists.update((items) => [created, ...items]);
        }
      });
  }

  visibilityLabel(visibility: ListVisibility): string {
    switch (visibility) {
      case ListVisibility.Public:
        return 'Public';
      case ListVisibility.Private:
        return 'Private';
      case ListVisibility.FriendsOnly:
        return 'Friends only';
      default:
        return 'Unknown';
    }
  }
}
