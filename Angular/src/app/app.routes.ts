import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./pages/login/login.component').then(m => m.LoginComponent)
  },

  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },

  {
    path: 'dashboard',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component').then(
        m => m.DashboardComponent
      )
  },

  {
    path: 'users',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/users/user-list/user-list.component').then(
        m => m.UserListComponent
      )
  },

  {
    path: 'users/create',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/users/create-user/create-user.component').then(
        m => m.CreateUserComponent
      )
  },

  {
    path: 'employees',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/employees/employee-list/employee-list.component').then(
        m => m.EmployeeListComponent
      )
  },

  {
    path: 'employees/add',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/employees/add-edit-employee/add-edit-employee.component').then(
        m => m.AddEditEmployeeComponent
      )
  },

  {
    path: 'employees/edit/:id',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/employees/add-edit-employee/add-edit-employee.component').then(
        m => m.AddEditEmployeeComponent
      )
  },

  {
    path: 'attendance',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/attendance/check-in/check-in.component').then(
        m => m.CheckInComponent
      )
  },

  {
    path: 'attendance/check-in',
    redirectTo: 'attendance',
    pathMatch: 'full'
  },

  {
    path: 'attendance/check-out',
    redirectTo: 'attendance',
    pathMatch: 'full'
  },

  {
    path: 'attendance/history',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/attendance/history/history.component').then(
        m => m.HistoryComponent
      )
  },

  {
    path: 'attendance/report',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./pages/attendance/report/report.component').then(
        m => m.ReportsComponent
      )
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
