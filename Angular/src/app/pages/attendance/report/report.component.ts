import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule
} from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';

import {
  AttendanceReportDTO,
  AttendanceReportResult,
  AttendanceService
} from '../../../core/services/attendance.service';
import {
  Employee,
  EmployeeService
} from '../../../core/services/employee.service';
import { EgyptDatePipe } from '../../../shared/egypt-date.pipe';
import { HoursMinutesPipe } from '../../../shared/hours-minutes.pipe';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatInputModule,
    EgyptDatePipe,
    HoursMinutesPipe
  ],
  templateUrl: './report.component.html',
  styleUrl: './report.component.scss'
})
export class ReportsComponent implements OnInit {

  private readonly attendanceService = inject(AttendanceService);
  private readonly employeeService = inject(EmployeeService);
  private readonly fb = inject(FormBuilder);

  employees: Employee[] = [];

  report: AttendanceReportResult | null = null;

  loading = false;

  displayedColumns = [
    'employee',
    'date',
    'checkIn',
    'checkOut',
    'workingHours'
  ];

  form = this.fb.group({
    employeeId: [null as number | null],
    from: [null as Date | null],
    to: [null as Date | null]
  });

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeService.getActive().subscribe({
      next: res => this.employees = res,
      error: err => console.error(err)
    });
  }

  search(): void {
    const employeeId = this.form.value.employeeId;
    if (!employeeId) {
      this.report = null;
      return;
    }

    this.loading = true;

    const payload: AttendanceReportDTO = {
      employeeId,
      from: this.toDateOnly(this.form.value.from),
      to: this.toDateOnly(this.form.value.to)
    };

    this.attendanceService.getReport(payload).subscribe({
      next: res => {
        this.report = res;
        this.loading = false;
      },
      error: err => {
        console.error(err);
        this.loading = false;
      }
    });

  }

  clear(): void {

    this.form.reset({
      employeeId: null,
      from: null,
      to: null
    });

    this.report = null;

  }

  private toDateOnly(date: Date | null | undefined): string | undefined {

    if (!date) {
      return undefined;
    }

    const year = date.getFullYear();
    const month = (date.getMonth() + 1)
      .toString()
      .padStart(2, '0');
    const day = date
      .getDate()
      .toString()
      .padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

}