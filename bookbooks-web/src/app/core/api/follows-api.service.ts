import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { FollowedUser } from '../models/followed-user.model';

@Injectable({ providedIn: 'root' })
export class FollowsApiService {
  private readonly httpClient = inject(HttpClient);

  getMine(): Observable<FollowedUser[]> {
    return this.httpClient.get<FollowedUser[]>('/api/follows/me');
  }

  follow(followedUserId: string): Observable<void> {
    return this.httpClient.post<void>(`/api/follows/${followedUserId}`, {});
  }

  unfollow(followedUserId: string): Observable<void> {
    return this.httpClient.delete<void>(`/api/follows/${followedUserId}`);
  }
}
