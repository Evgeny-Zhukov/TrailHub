import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-card">
      <h1>Регистрация</h1>
      <p class="subtitle">Присоединяйся к сообществу Trail</p>

      <form [formGroup]="form" (ngSubmit)="submit()">
        <label>
          Имя пользователя
          <input type="text" formControlName="username" placeholder="hiker_2026" />
        </label>

        <label>
          Email
          <input type="email" formControlName="email" placeholder="you@mail.ru" />
        </label>

        <label>
          Пароль
          <input type="password" formControlName="password" />
          @if (form.get('password')?.hasError('minlength')) {
            <span class="error">Минимум 6 символов</span>
          }
        </label>

        @if (error()) {
          <div class="error-banner">{{ error() }}</div>
        }

        <button type="submit" [disabled]="form.invalid || loading()">
          {{ loading() ? 'Создаём аккаунт…' : 'Зарегистрироваться' }}
        </button>
      </form>

      <p class="switch">
        Уже есть аккаунт? <a routerLink="/login">Войти</a>
      </p>
    </div>
  `,
  styleUrl: './register.scss',
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal('');

  readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  submit(): void {
    if (this.form.invalid || this.loading()) return;

    this.loading.set(true);
    this.error.set('');

    this.authService.register(this.form.getRawValue()).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (err) => {
        this.loading.set(false);
        this.error.set('Не удалось зарегистрироваться. Попробуйте ещё раз.');
      },
    });
  }
}