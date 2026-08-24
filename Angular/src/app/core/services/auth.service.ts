import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface AccessToken {
  token: string;
  expirationTime: string;
}

export interface AccessTokenResponse {
  success: boolean;
  message: string;
  token: AccessToken | null;
}

export interface LoginResult {
  loginResponse: AccessTokenResponse;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly tokenKey = 'attendance.accessToken';
  private readonly refreshKey = 'attendance.refreshToken';

  login(username: string, password: string) {
    return this.http
      .post<LoginResult>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(
        tap(result => {
          const token = result.loginResponse?.token?.token;
          if (token) {
            localStorage.setItem(this.tokenKey, token);
          }
          if (result.refreshToken) {
            localStorage.setItem(this.refreshKey, result.refreshToken);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshKey);
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }
}
