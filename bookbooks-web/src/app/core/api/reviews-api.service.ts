import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateReviewRequest } from '../models/create-review-request.model';
import { Review } from '../models/review.model';

@Injectable({ providedIn: 'root' })
export class ReviewsApiService {
  private readonly httpClient = inject(HttpClient);

  getByBook(bookId: string, page = 1, pageSize = 20): Observable<Review[]> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    return this.httpClient.get<Review[]>(`/api/books/${bookId}/reviews`, { params });
  }

  create(bookId: string, request: CreateReviewRequest): Observable<{ reviewId: string }> {
    return this.httpClient.post<{ reviewId: string }>(`/api/books/${bookId}/reviews`, request);
  }
}
