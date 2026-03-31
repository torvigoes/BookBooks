import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Book } from '../models/book.model';
import { CreateBookRequest } from '../models/create-book-request.model';

@Injectable({ providedIn: 'root' })
export class BooksApiService {
  private readonly httpClient = inject(HttpClient);

  search(searchTerm: string, page = 1, pageSize = 20): Observable<Book[]> {
    const params = new HttpParams()
      .set('searchTerm', searchTerm)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.httpClient.get<Book[]>('/api/books', { params });
  }

  getById(bookId: string): Observable<Book> {
    return this.httpClient.get<Book>(`/api/books/${bookId}`);
  }

  create(request: CreateBookRequest): Observable<string> {
    return this.httpClient.post<string>('/api/books', request);
  }
}
