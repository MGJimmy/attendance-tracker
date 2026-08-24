import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

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
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component').then(
        m => m.DashboardComponent
      )
  },

  {
    path: 'users/create',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/users/create-user/create-user.component').then(
        m => m.CreateUserComponent
      )
  },

  {
    path: 'employees',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/employees/employee-list/employee-list.component').then(
        m => m.EmployeeListComponent
      )
  },

  {
    path: 'employees/add',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/employees/add-edit-employee/add-edit-employee.component').then(
        m => m.AddEditEmployeeComponent
      )
  },

  {
    path: 'employees/edit/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/employees/add-edit-employee/add-edit-employee.component').then(
        m => m.AddEditEmployeeComponent
      )
  },

  {
    path: 'attendance/check-in',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/attendance/check-in/check-in.component').then(
        m => m.CheckInComponent
      )
  },

  {
    path: 'attendance/check-out',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/attendance/check-out/check-out.component').then(
        m => m.CheckOutComponent
      )
  },

  {
    path: 'attendance/history',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/attendance/history/history.component').then(
        m => m.HistoryComponent
      )
  },

  {
    path: 'attendance/report',
    canActivate: [authGuard],
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
