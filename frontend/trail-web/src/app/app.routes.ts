import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'trails' },

  // Публичные
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register').then((m) => m.RegisterComponent),
  },

//   // Защищённые (пример)
//   {
//     path: 'profile',
//     canActivate: [authGuard],
//     loadChildren: () =>
//       import('./features/profile/profile.routes').then((m) => m.PROFILE_ROUTES),
//   },

  { path: '**', redirectTo: 'trails' },
];