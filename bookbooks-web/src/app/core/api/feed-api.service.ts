import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { FeedItem } from '../models/feed-item.model';

@Injectable({ providedIn: 'root' })
export class FeedApiService {
  private readonly httpClient = inject(HttpClient);

  getMine(page = 1, pageSize = 20): Observable<FeedItem[]> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    return this.httpClient.get<FeedItem[]>('/api/feed', { params });
  }
}
