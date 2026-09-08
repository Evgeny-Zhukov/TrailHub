import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginDto,
  RegisterDto,
  UserProfile,
} from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly TOKEN_KEY = 'trail_token';
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  // Реактивное состояние через signal (современный подход)
  private readonly user = signal<UserProfile | null>(null);
  readonly currentUser = this.user.asReadonly();
  readonly isAuthenticated = computed(() => !!this.user() && !!this.token);

  constructor() {
    // При старте приложения проверяем, есть ли сохранённый токен
    if (this.token) {
      this.fetchProfile().subscribe();
    }
  }

  get token(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  login(dto: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, dto).pipe(
      tap((res) => this.handleAuth(res)),
    );
  }

  register(dto: RegisterDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, dto).pipe(
      tap((res) => this.handleAuth(res)),
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.user.set(null);
    this.router.navigate(['/login']);
  }

  private handleAuth(res: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, res.token);
    this.user.set(res.user);
  }

  private fetchProfile(): Observable<UserProfile> {
    return this.http
      .get<UserProfile>(`${this.apiUrl}/profile`)
      .pipe(tap((user) => this.user.set(user)));
  }
}