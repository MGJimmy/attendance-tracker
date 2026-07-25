import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { Employee, EmployeeService } from '../../../core/services/employee.service';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule
  ],
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.scss'
})
export class EmployeeListComponent implements OnInit {

  private readonly employeeService = inject(EmployeeService);
  private readonly router = inject(Router);

  employees: Employee[] = [];

  displayedColumns: string[] = [
    'name',
    'salary',
    'status',
    'actions'
  ];

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeService.getAll().subscribe({
      next: (response) => {
        this.employees = response;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  edit(id: number): void {
    this.router.navigate(['/employees/edit', id]);
  }

  activate(id: number): void {
    this.employeeService.activate(id).subscribe({
      next: () => {
        this.loadEmployees();
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  deactivate(id: number): void {
    this.employeeService.deactivate(id).subscribe({
      next: () => {
        this.loadEmployees();
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

}