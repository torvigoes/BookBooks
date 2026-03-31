import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { LoginPage } from './features/auth/login/login.page';
import { RegisterPage } from './features/auth/register/register.page';
import { BookDetailsPage } from './features/books/book-details.page';
import { BooksPage } from './features/books/books.page';
import { FollowsPage } from './features/follows/follows.page';
import { HomePage } from './features/home/home.page';
import { ListsPage } from './features/lists/lists.page';

export const routes: Routes = [
  { path: '', component: HomePage, pathMatch: 'full' },
  { path: 'login', component: LoginPage },
  { path: 'register', component: RegisterPage },
  { path: 'books', component: BooksPage },
  { path: 'books/:id', component: BookDetailsPage },
  { path: 'lists', component: ListsPage, canActivate: [authGuard] },
  { path: 'follows', component: FollowsPage, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
