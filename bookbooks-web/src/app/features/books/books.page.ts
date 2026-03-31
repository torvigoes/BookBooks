import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { BooksApiService } from '../../core/api/books-api.service';
import { HttpErrorService } from '../../core/api/http-error.service';
import { Book } from '../../core/models/book.model';

@Component({
  selector: 'app-books-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <section class="grid">
      <article class="card">
        <h1>Search books</h1>
        <form [formGroup]="searchForm" (ngSubmit)="search()">
          <input type="text" formControlName="searchTerm" placeholder="Title or author" />
          <button type="submit" [disabled]="isSearching()">Search</button>
        </form>

        @if (isSearching()) {
          <p class="muted">Searching...</p>
        }

        @if (!isSearching() && books().length === 0) {
          <p class="muted">No books found yet.</p>
        }

        <ul class="book-list">
          @for (book of books(); track book.id) {
            <li>
              <div>
                <strong>{{ book.title }}</strong>
                <p>{{ book.author }} ({{ book.year }})</p>
              </div>
              <a [routerLink]="['/books', book.id]">Details</a>
            </li>
          }
        </ul>
      </article>

      <article class="card">
        <h2>Create book</h2>
        <form [formGroup]="createForm" (ngSubmit)="create()">
          <input type="text" formControlName="title" placeholder="Title" />
          <input type="text" formControlName="author" placeholder="Author" />
          <input type="text" formControlName="isbn" placeholder="ISBN (10-13)" />
          <input type="number" formControlName="year" placeholder="Year" />
          <input type="text" formControlName="coverImageUrl" placeholder="Cover URL (optional)" />

          <button type="submit" [disabled]="isCreating() || createForm.invalid">
            {{ isCreating() ? 'Creating...' : 'Create' }}
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
    }

    button:disabled {
      cursor: not-allowed;
      opacity: 0.7;
    }

    .book-list {
      list-style: none;
      margin: 0.8rem 0 0;
      padding: 0;
    }

    .book-list li {
      align-items: center;
      border-top: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      padding: 0.7rem 0;
    }

    .book-list p {
      color: #475569;
      margin: 0.15rem 0 0;
    }

    .muted {
      color: #64748b;
    }
  `
})
export class BooksPage {
  private readonly booksApi = inject(BooksApiService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly httpErrorService = inject(HttpErrorService);
  private readonly router = inject(Router);

  readonly isSearching = signal(false);
  readonly isCreating = signal(false);
  readonly books = signal<Book[]>([]);

  readonly searchForm = this.formBuilder.nonNullable.group({
    searchTerm: ['']
  });

  readonly createForm = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    author: ['', [Validators.required, Validators.maxLength(150)]],
    isbn: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1)]],
    coverImageUrl: ['']
  });

  constructor() {
    this.search();
  }

  search(): void {
    if (this.isSearching()) {
      return;
    }

    this.httpErrorService.clear();
    const searchTerm = this.searchForm.getRawValue().searchTerm;

    this.isSearching.set(true);
    this.booksApi
      .search(searchTerm)
      .pipe(finalize(() => this.isSearching.set(false)))
      .subscribe({
        next: (items) => this.books.set(items)
      });
  }

  create(): void {
    if (this.createForm.invalid || this.isCreating()) {
      return;
    }

    this.httpErrorService.clear();
    const raw = this.createForm.getRawValue();

    this.isCreating.set(true);
    this.booksApi
      .create({
        title: raw.title,
        author: raw.author,
        isbn: raw.isbn,
        year: raw.year,
        coverImageUrl: raw.coverImageUrl.trim() ? raw.coverImageUrl.trim() : null
      })
      .pipe(finalize(() => this.isCreating.set(false)))
      .subscribe({
        next: (bookId) => this.router.navigate(['/books', bookId])
      });
  }
}
