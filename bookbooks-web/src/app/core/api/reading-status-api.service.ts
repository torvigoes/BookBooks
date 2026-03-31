import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, of, throwError } from 'rxjs';
import { HttpErrorService } from './http-error.service';
import { ReadingStatus } from '../models/reading-status.model';
import { UpdateReadingStatusRequest } from '../models/update-reading-status-request.model';

@Injectable({ providedIn: 'root' })
export class ReadingStatusApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly httpErrorService = inject(HttpErrorService);

  getByBook(bookId: string): Observable<ReadingStatus | null> {
    return this.httpClient.get<ReadingStatus>(`/api/books/${bookId}/reading-status`).pipe(
      catchError((error: unknown) => {
        if (error instanceof HttpErrorResponse && error.status === 404) {
          this.httpErrorService.clear();
          return of(null);
        }

        return throwError(() => error);
      })
    );
  }

  upsert(bookId: string, request: UpdateReadingStatusRequest): Observable<ReadingStatus> {
    return this.httpClient.put<ReadingStatus>(`/api/books/${bookId}/reading-status`, request);
  }
}
