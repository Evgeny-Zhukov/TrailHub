import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-card">
      <h1>Вход в Trail</h1>
      <p class="subtitle">Найди свой следующий маршрут</p>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <label>
          Email
          <input type="email" formControlName="email" placeholder="you@mail.ru" />
          @if (form.get('email')?.hasError('email')) {
            <span class="error">Некорректный email</span>
          }
        </label>

        <label>
          Пароль
          <input type="password" formControlName="password" />
        </label>

        @if (error()) {
          <div class="error-banner">{{ error() }}</div>
        }

        <button type="submit" [disabled]="form.invalid || loading()">
          {{ loading() ? 'Входим…' : 'Войти' }}
        </button>
      </form>

      <p class="switch">
        Нет аккаунта? <a routerLink="/register">Зарегистрироваться</a>
      </p>
    </div>
  `,
  styleUrl: './login.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal('');

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  submit(): void {
    if (this.form.invalid || this.loading()) return;

    this.loading.set(true);
    this.error.set('');

    this.authService.login(this.form.getRawValue()).subscribe({
      next: () => {
        const returnUrl =
          this.router.parseUrl(this.router.url).queryParams['returnUrl'];
        this.router.navigateByUrl(returnUrl ?? '/');
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Неверный email или пароль');
      },
    });
  }
}