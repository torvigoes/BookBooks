import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AddBookToListRequest } from '../models/add-book-to-list-request.model';
import { BookList } from '../models/book-list.model';
import { CreateListRequest } from '../models/create-list-request.model';

@Injectable({ providedIn: 'root' })
export class ListsApiService {
  private readonly httpClient = inject(HttpClient);

  getMine(): Observable<BookList[]> {
    return this.httpClient.get<BookList[]>('/api/lists');
  }

  getById(listId: string): Observable<BookList> {
    return this.httpClient.get<BookList>(`/api/lists/${listId}`);
  }

  create(request: CreateListRequest): Observable<BookList> {
    return this.httpClient.post<BookList>('/api/lists', request);
  }

  addBook(listId: string, request: AddBookToListRequest): Observable<BookList> {
    return this.httpClient.post<BookList>(`/api/lists/${listId}/books`, request);
  }

  removeBook(listId: string, bookId: string): Observable<BookList> {
    return this.httpClient.delete<BookList>(`/api/lists/${listId}/books/${bookId}`);
  }
}
