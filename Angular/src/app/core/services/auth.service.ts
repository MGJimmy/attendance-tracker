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

  getUserId(): string | null {
    return this.readClaim('nameIdentifier') ?? this.readClaim('nameid');
  }

  getRoles(): string[] {
    const raw =
      this.readClaim('roles') ??
      this.readClaim('role') ??
      this.readClaim('http://schemas.microsoft.com/ws/2008/06/identity/claims/role');

    if (!raw) {
      return [];
    }

    if (Array.isArray(raw)) {
      return raw.map(role => String(role));
    }

    try {
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed.map(role => String(role)) : [String(parsed)];
    } catch {
      return [String(raw)];
    }
  }

  isAdmin(): boolean {
    return this.getRoles().some(role => role.toLowerCase() === 'admin');
  }

  homeUrl(): string {
    return this.isAdmin() ? '/dashboard' : '/attendance';
  }

  private readClaim(key: string): any {
    const payload = this.decodeToken();
    return payload?.[key];
  }

  private decodeToken(): any | null {
    const token = this.getAccessToken();
    if (!token) {
      return null;
    }

    try {
      const payload = token.split('.')[1];
      const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(json);
    } catch {
      return null;
    }
  }
}
