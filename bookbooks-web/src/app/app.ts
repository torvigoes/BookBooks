import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { HttpErrorService } from './core/api/http-error.service';
import { AuthSessionService } from './core/auth/auth-session.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly authSession = inject(AuthSessionService);
  protected readonly httpErrorService = inject(HttpErrorService);

  protected clearHttpError(): void {
    this.httpErrorService.clear();
  }

  protected signOut(): void {
    this.authSession.signOut();
  }
}
