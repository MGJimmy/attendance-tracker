import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule
} from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';

import {
  Attendance,
  AttendanceService
} from '../../../core/services/attendance.service';

import {
  Employee,
  EmployeeService
} from '../../../core/services/employee.service';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule
  ],
  templateUrl: './history.component.html',
  styleUrl: './history.component.scss'
})
export class HistoryComponent implements OnInit {

  private readonly attendanceService = inject(AttendanceService);
  private readonly employeeService = inject(EmployeeService);
  private readonly fb = inject(FormBuilder);

  employees: Employee[] = [];
  attendance: Attendance[] = [];
  loading = false;

  displayedColumns: string[] = [
    'attendanceDate',
    'name',
    'checkInTime',
    'checkOutTime',
    'workingHours'
  ];

  form = this.fb.group({
    employeeId: [null as number | null],
    from: [null as Date | null],
    to: [null as Date | null]
  });

  ngOnInit(): void {
    this.loadEmployees();
    this.search();
  }

  //#region Load Employees

  loadEmployees(): void {

    this.employeeService.getActive().subscribe({
      next: (res) => {
        this.employees = res;
      },
      error: (err) => console.error(err)
    });

  }

  //#endregion

  //#region Search

  search(): void {

    this.loading = true;

    const from = this.form.value.from;
    const to = this.form.value.to;

    this.attendanceService.getHistory({

      employeeId: this.form.value.employeeId ?? undefined,

      from: from ? this.toDateOnly(from) : undefined,

      to: to ? this.toDateOnly(to) : undefined

    }).subscribe({

      next: (res) => {
        this.attendance = res;
        this.loading = false;
      },

      error: (err) => {
        console.error(err);
        this.loading = false;
      }

    });

  }

  //#endregion

  //#region Clear

  clear(): void {

    this.form.reset();

    this.attendance = [];

  }

  //#endregion

  private toDateOnly(date: Date | null): string | undefined {
  if (!date) {
    return undefined;
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
}

}