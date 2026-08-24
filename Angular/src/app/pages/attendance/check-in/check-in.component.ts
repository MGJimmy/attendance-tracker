import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { Attendance, AttendanceService, CheckInDTO } from '../../../core/services/attendance.service';
import { Employee, EmployeeService } from '../../../core/services/employee.service';
import { AuthService } from '../../../core/services/auth.service';
import { DATE_TIME_FORMAT } from '../../../shared/date-formats';
import { EgyptDatePipe } from '../../../shared/egypt-date.pipe';
import { HoursMinutesPipe } from '../../../shared/hours-minutes.pipe';
import { toEgyptDateTimeInput } from '../../../shared/date-formats';

@Component({
  selector: 'app-check-in',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    EgyptDatePipe,
    HoursMinutesPipe
  ],
  templateUrl: './check-in.component.html',
  styleUrl: './check-in.component.scss'
})
export class CheckInComponent implements OnInit {
  private readonly attendanceService = inject(AttendanceService);
  private readonly employeeService = inject(EmployeeService);
  private readonly fb = inject(FormBuilder);
  readonly auth = inject(AuthService);

  readonly dateTimeFormat = DATE_TIME_FORMAT;
  readonly isAdmin = this.auth.isAdmin();

  employees: Employee[] = [];
  currentEmployee: Employee | null = null;
  todayAttendance: Attendance[] = [];
  errorMessage = '';
  loadError = '';

  form = this.fb.group({
    employeeId: [null as number | null],
    occurredAt: [toEgyptDateTimeInput()]
  });

  ngOnInit(): void {
    if (this.isAdmin) {
      this.form.controls.employeeId.addValidators(Validators.required);
      this.loadEmployees();
    } else {
      this.loadCurrentEmployee();
    }

    this.loadTodayAttendance();
  }

  loadEmployees(): void {
    this.employeeService.getActive().subscribe({
      next: res => {
        this.employees = res;
      },
      error: err => console.error(err)
    });
  }

  loadCurrentEmployee(): void {
    this.employeeService.getCurrent().subscribe({
      next: employee => {
        this.currentEmployee = employee;
        this.form.patchValue({ employeeId: employee.id });
      },
      error: (err: HttpErrorResponse) => {
        this.loadError = this.readError(err) || 'No employee is linked to this user.';
      }
    });
  }

  loadTodayAttendance(): void {
    this.attendanceService.getTodayAttendance().subscribe({
      next: res => {
        this.todayAttendance = res;
      },
      error: err => console.error(err)
    });
  }

  checkIn(): void {
    this.submit('in');
  }

  checkOut(): void {
    this.submit('out');
  }

  getSelectedAttendance(): Attendance | null {
    const employeeId = this.form.value.employeeId ?? this.currentEmployee?.id;
    if (!employeeId) {
      return null;
    }

    return this.todayAttendance.find(x => x.employeeId === employeeId) ?? null;
  }

  private submit(action: 'in' | 'out'): void {
    if (this.form.invalid || (!this.isAdmin && !this.currentEmployee)) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage = '';

    const payload: CheckInDTO = {};
    if (this.isAdmin) {
      payload.employeeId = this.form.value.employeeId!;
      payload.occurredAt = this.form.value.occurredAt || undefined;
    }

    const request = action === 'in'
      ? this.attendanceService.checkIn(payload)
      : this.attendanceService.checkOut(payload);

    request.subscribe({
      next: () => {
        this.loadTodayAttendance();
        if (this.isAdmin) {
          this.form.patchValue({ occurredAt: toEgyptDateTimeInput() });
        }
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage = this.readError(err) ||
          (action === 'in' ? 'Could not check in.' : 'Could not check out.');
      }
    });
  }

  private readError(err: HttpErrorResponse): string {
    const body = err.error;
    if (typeof body === 'string') {
      return body;
    }
    return body?.message || body?.title || '';
  }
}
