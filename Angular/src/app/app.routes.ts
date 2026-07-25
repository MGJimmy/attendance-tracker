import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },

  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component').then(
        m => m.DashboardComponent
      )
  },

  {
    path: 'employees',
    loadComponent: () =>
      import('./pages/employees/employee-list/employee-list.component').then(
        m => m.EmployeeListComponent
      )
  },

  {
    path: 'employees/add',
    loadComponent: () =>
      import('./pages/employees/add-edit-employee/add-edit-employee.component').then(
        m => m.AddEditEmployeeComponent
      )
  },

  {
    path: 'employees/edit/:id',
    loadComponent: () =>
      import('./pages/employees/add-edit-employee/add-edit-employee.component').then(
        m => m.AddEditEmployeeComponent
      )
  },

  {
    path: 'attendance/check-in',
    loadComponent: () =>
      import('./pages/attendance/check-in/check-in.component').then(
        m => m.CheckInComponent
      )
  },

  {
    path: 'attendance/check-out',
    loadComponent: () =>
      import('./pages/attendance/check-out/check-out.component').then(
        m => m.CheckOutComponent
      )
  },

  {
    path: 'attendance/history',
    loadComponent: () =>
      import('./pages/attendance/history/history.component').then(
        m => m.HistoryComponent
      )
  },

  {
    path: 'attendance/report',
    loadComponent: () =>
      import('./pages/attendance/report/report.component').then(
        m => m.ReportComponent
      )
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }
];