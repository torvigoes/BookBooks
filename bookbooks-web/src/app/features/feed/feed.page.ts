import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { FeedApiService } from '../../core/api/feed-api.service';
import { HttpErrorService } from '../../core/api/http-error.service';
import { FeedItem } from '../../core/models/feed-item.model';

@Component({
  selector: 'app-feed-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <section class="feed-page">
      <header>
        <h1>Feed</h1>
        <p>Latest reviews from people you follow.</p>
      </header>

      @if (isLoading()) {
        <p class="muted">Loading feed...</p>
      } @else if (items().length === 0) {
        <p class="muted">
          Your feed is empty. Follow more users to see their reviews here.
        </p>
      } @else {
        <ul class="feed-list">
          @for (item of items(); track item.reviewId) {
            <li class="feed-item">
              <div class="meta">
                <strong>{{ item.userDisplayName || item.userId }}</strong>
                <span>reviewed</span>
                <a [routerLink]="['/books', item.bookId]">{{ item.bookTitle || item.bookId }}</a>
              </div>

              <p class="book-author">{{ item.bookAuthor }}</p>
              <p class="rating">Rating: {{ item.rating }}/5</p>

              @if (item.containsSpoiler) {
                <p class="spoiler">Contains spoiler</p>
              }

              <p class="content">{{ item.content }}</p>
              <small>{{ formatDate(item.createdAt) }}</small>
            </li>
          }
        </ul>
      }
    </section>
  `,
  styles: `
    .feed-page {
      display: grid;
      gap: 1rem;
      max-width: 840px;
    }

    h1 {
      margin: 0;
    }

    header p {
      color: #64748b;
      margin: 0.3rem 0 0;
    }

    .muted {
      color: #64748b;
    }

    .feed-list {
      display: grid;
      gap: 0.85rem;
      list-style: none;
      margin: 0;
      padding: 0;
    }

    .feed-item {
      background: #fff;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      display: grid;
      gap: 0.45rem;
      padding: 1rem;
    }

    .meta {
      align-items: center;
      display: flex;
      flex-wrap: wrap;
      gap: 0.35rem;
    }

    .meta a {
      color: #1d4ed8;
      text-decoration: none;
    }

    .book-author {
      color: #475569;
      margin: 0;
    }

    .rating {
      font-weight: 600;
      margin: 0;
    }

    .spoiler {
      color: #b45309;
      font-weight: 600;
      margin: 0;
    }

    .content {
      margin: 0;
      white-space: pre-wrap;
    }

    small {
      color: #64748b;
    }
  `
})
export class FeedPage {
  private readonly feedApi = inject(FeedApiService);
  private readonly httpErrorService = inject(HttpErrorService);

  readonly isLoading = signal(false);
  readonly items = signal<FeedItem[]>([]);

  constructor() {
    this.load();
  }

  load(): void {
    this.httpErrorService.clear();
    this.isLoading.set(true);

    this.feedApi
      .getMine()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (items) => this.items.set(items)
      });
  }

  formatDate(isoValue: string): string {
    return new Date(isoValue).toLocaleString();
  }
}
