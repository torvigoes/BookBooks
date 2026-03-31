import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ReviewsApiService } from '../../core/api/reviews-api.service';
import { BooksApiService } from '../../core/api/books-api.service';
import { HttpErrorService } from '../../core/api/http-error.service';
import { ReadingStatusApiService } from '../../core/api/reading-status-api.service';
import { AuthSessionService } from '../../core/auth/auth-session.service';
import { Book } from '../../core/models/book.model';
import { ReadingStatusType } from '../../core/models/reading-status-type.enum';
import { Review } from '../../core/models/review.model';

@Component({
  selector: 'app-book-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    @if (!book()) {
      <p class="muted">Loading book...</p>
    } @else {
      <article class="card">
        <a routerLink="/books">Back to books</a>
        <h1>{{ book()!.title }}</h1>
        <p><strong>Author:</strong> {{ book()!.author }}</p>
        <p><strong>ISBN:</strong> {{ book()!.isbn }}</p>
        <p><strong>Year:</strong> {{ book()!.year }}</p>
        <p>
          <strong>Rating:</strong>
          {{ book()!.averageRating.toFixed(2) }} ({{ book()!.reviewCount }} reviews)
        </p>
      </article>

      <section class="card">
        <h2>Reviews</h2>
        @if (isLoadingReviews()) {
          <p class="muted">Loading reviews...</p>
        } @else if (reviews().length === 0) {
          <p class="muted">No reviews yet.</p>
        } @else {
          <ul class="review-list">
            @for (review of reviews(); track review.id) {
              <li>
                <p><strong>{{ review.userDisplayName }}</strong> - {{ review.rating }}/5</p>
                <p>{{ review.content }}</p>
                @if (review.containsSpoiler) {
                  <p class="spoiler">Contains spoiler</p>
                }
              </li>
            }
          </ul>
        }
      </section>

      @if (authSession.isAuthenticated()) {
        <section class="card">
          <h3>Reading status</h3>
          <form [formGroup]="readingStatusForm" (ngSubmit)="saveReadingStatus()">
            <select formControlName="status">
              <option [ngValue]="readingStatusType.WantToRead">Want to read</option>
              <option [ngValue]="readingStatusType.CurrentlyReading">Currently reading</option>
              <option [ngValue]="readingStatusType.Read">Read</option>
              <option [ngValue]="readingStatusType.Abandoned">Abandoned</option>
            </select>
            <button type="submit" [disabled]="isSavingReadingStatus()">
              {{ isSavingReadingStatus() ? 'Saving...' : 'Save status' }}
            </button>
          </form>

          @if (currentReadingStatusUpdatedAt()) {
            <p class="muted">Last updated: {{ currentReadingStatusUpdatedAt() }}</p>
          }
        </section>

        <section class="card">
          <h3>Add review</h3>
          <form [formGroup]="reviewForm" (ngSubmit)="createReview()">
            <input type="number" formControlName="rating" min="1" max="5" placeholder="Rating (1-5)" />
            <textarea rows="4" formControlName="content" placeholder="Write your review"></textarea>
            <label class="check">
              <input type="checkbox" formControlName="containsSpoiler" />
              Contains spoiler
            </label>
            <button type="submit" [disabled]="isSubmittingReview() || reviewForm.invalid">
              {{ isSubmittingReview() ? 'Submitting...' : 'Submit review' }}
            </button>
          </form>
        </section>
      } @else {
        <p class="muted">Login to write a review.</p>
      }
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

    .review-list {
      list-style: none;
      margin: 0;
      padding: 0;
    }

    .review-list li {
      border-top: 1px solid #e2e8f0;
      padding: 0.7rem 0;
    }

    .review-list p {
      margin: 0.2rem 0;
    }

    .spoiler {
      color: #b45309;
      font-size: 0.9rem;
      font-style: italic;
    }

    form {
      display: grid;
      gap: 0.6rem;
    }

    input,
    textarea,
    select {
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      padding: 0.55rem 0.7rem;
    }

    .check {
      align-items: center;
      display: inline-flex;
      gap: 0.45rem;
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

    button:disabled {
      cursor: not-allowed;
      opacity: 0.7;
    }
  `
})
export class BookDetailsPage {
  private readonly booksApi = inject(BooksApiService);
  private readonly reviewsApi = inject(ReviewsApiService);
  private readonly readingStatusApi = inject(ReadingStatusApiService);
  readonly authSession = inject(AuthSessionService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly httpErrorService = inject(HttpErrorService);

  readonly book = signal<Book | null>(null);
  readonly reviews = signal<Review[]>([]);
  readonly isLoadingReviews = signal(false);
  readonly isSubmittingReview = signal(false);
  readonly isSavingReadingStatus = signal(false);
  readonly currentReadingStatusUpdatedAt = signal<string | null>(null);
  readonly readingStatusType = ReadingStatusType;

  readonly reviewForm = this.formBuilder.nonNullable.group({
    rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    content: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(5000)]],
    containsSpoiler: [false]
  });

  readonly readingStatusForm = this.formBuilder.nonNullable.group({
    status: [ReadingStatusType.WantToRead, [Validators.required]]
  });

  constructor() {
    const bookId = this.route.snapshot.paramMap.get('id');
    if (!bookId) {
      return;
    }

    this.loadBook(bookId);
    this.loadReviews(bookId);
    if (this.authSession.isAuthenticated()) {
      this.loadReadingStatus(bookId);
    }
  }

  private loadBook(bookId: string): void {
    this.httpErrorService.clear();
    this.booksApi.getById(bookId).subscribe({
      next: (item) => this.book.set(item)
    });
  }

  private loadReviews(bookId: string): void {
    this.isLoadingReviews.set(true);
    this.reviewsApi
      .getByBook(bookId)
      .pipe(finalize(() => this.isLoadingReviews.set(false)))
      .subscribe({
        next: (items) => this.reviews.set(items)
      });
  }

  private loadReadingStatus(bookId: string): void {
    this.readingStatusApi.getByBook(bookId).subscribe({
      next: (status) => {
        if (!status) {
          this.currentReadingStatusUpdatedAt.set(null);
          this.readingStatusForm.patchValue({ status: ReadingStatusType.WantToRead });
          return;
        }

        this.currentReadingStatusUpdatedAt.set(new Date(status.updatedAt).toLocaleString());
        this.readingStatusForm.patchValue({ status: status.status });
      }
    });
  }

  createReview(): void {
    const bookId = this.route.snapshot.paramMap.get('id');
    if (!bookId || this.reviewForm.invalid || this.isSubmittingReview()) {
      return;
    }

    this.httpErrorService.clear();
    const raw = this.reviewForm.getRawValue();

    this.isSubmittingReview.set(true);
    this.reviewsApi
      .create(bookId, raw)
      .pipe(finalize(() => this.isSubmittingReview.set(false)))
      .subscribe({
        next: () => {
          this.reviewForm.patchValue({
            rating: 5,
            content: '',
            containsSpoiler: false
          });
          this.loadBook(bookId);
          this.loadReviews(bookId);
        }
      });
  }

  saveReadingStatus(): void {
    const bookId = this.route.snapshot.paramMap.get('id');
    if (!bookId || this.isSavingReadingStatus()) {
      return;
    }

    this.httpErrorService.clear();
    const raw = this.readingStatusForm.getRawValue();

    this.isSavingReadingStatus.set(true);
    this.readingStatusApi
      .upsert(bookId, { status: raw.status })
      .pipe(finalize(() => this.isSavingReadingStatus.set(false)))
      .subscribe({
        next: (result) => {
          this.currentReadingStatusUpdatedAt.set(new Date(result.updatedAt).toLocaleString());
        }
      });
  }
}
