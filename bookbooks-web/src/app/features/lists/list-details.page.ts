import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HttpErrorService } from '../../core/api/http-error.service';
import { ListsApiService } from '../../core/api/lists-api.service';
import { BookList } from '../../core/models/book-list.model';

@Component({
  selector: 'app-list-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    @if (!list()) {
      <p class="muted">Loading list...</p>
    } @else {
      <article class="card">
        <a routerLink="/lists">Back to lists</a>
        <h1>{{ list()!.name }}</h1>
        <p>{{ list()!.description || 'No description' }}</p>
        <p class="muted">{{ list()!.itemCount }} books in this list</p>
      </article>

      <section class="card">
        <h2>Add book by ID</h2>
        <form [formGroup]="form" (ngSubmit)="addBook()">
          <input type="text" formControlName="bookId" placeholder="Book ID" />
          <textarea rows="3" formControlName="notes" placeholder="Notes (optional)"></textarea>
          <button type="submit" [disabled]="isSubmitting() || form.invalid">
            {{ isSubmitting() ? 'Adding...' : 'Add book' }}
          </button>
        </form>
      </section>

      <section class="card">
        <h2>Books</h2>
        @if (list()!.items.length === 0) {
          <p class="muted">No books yet.</p>
        } @else {
          <ul class="book-items">
            @for (item of list()!.items; track item.bookId) {
              <li>
                <div>
                  <strong>{{ item.bookTitle }}</strong>
                  <p>{{ item.bookAuthor }}</p>
                  @if (item.notes) {
                    <small>{{ item.notes }}</small>
                  }
                </div>
                <button type="button" class="danger" [disabled]="isSubmitting()" (click)="removeBook(item.bookId)">
                  Remove
                </button>
              </li>
            }
          </ul>
        }
      </section>
    }
  `,
  styles: `
    .card {
      background: #fff;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      margin-bottom: 1rem;
      padding: 1rem;
    }

    .muted {
      color: #64748b;
    }

    form {
      display: grid;
      gap: 0.55rem;
    }

    input,
    textarea {
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

    .book-items {
      list-style: none;
      margin: 0;
      padding: 0;
    }

    .book-items li {
      align-items: center;
      border-top: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      padding: 0.7rem 0;
    }

    .book-items p {
      color: #475569;
      margin: 0.15rem 0;
    }

    .book-items small {
      color: #64748b;
    }
  `
})
export class ListDetailsPage {
  private readonly listsApi = inject(ListsApiService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly httpErrorService = inject(HttpErrorService);

  readonly list = signal<BookList | null>(null);
  readonly isSubmitting = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    bookId: ['', [Validators.required]],
    notes: ['']
  });

  constructor() {
    const listId = this.route.snapshot.paramMap.get('id');
    if (!listId) {
      this.router.navigate(['/lists']);
      return;
    }

    this.load(listId);
  }

  private get listId(): string | null {
    return this.route.snapshot.paramMap.get('id');
  }

  load(listId: string): void {
    this.httpErrorService.clear();
    this.listsApi.getById(listId).subscribe({
      next: (item) => this.list.set(item),
      error: () => {
        if (this.httpErrorService.message()?.includes('401')) {
          this.router.navigate(['/login']);
        }
      }
    });
  }

  addBook(): void {
    const listId = this.listId;
    if (!listId || this.form.invalid || this.isSubmitting()) {
      return;
    }

    this.httpErrorService.clear();
    const raw = this.form.getRawValue();

    this.isSubmitting.set(true);
    this.listsApi
      .addBook(listId, {
        bookId: raw.bookId,
        notes: raw.notes.trim() ? raw.notes.trim() : null
      })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (updated) => {
          this.list.set(updated);
          this.form.patchValue({
            bookId: '',
            notes: ''
          });
        }
      });
  }

  removeBook(bookId: string): void {
    const listId = this.listId;
    if (!listId || this.isSubmitting()) {
      return;
    }

    this.httpErrorService.clear();
    this.isSubmitting.set(true);
    this.listsApi
      .removeBook(listId, bookId)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (updated) => this.list.set(updated)
      });
  }
}
