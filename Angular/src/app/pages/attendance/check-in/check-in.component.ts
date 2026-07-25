import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Attendance, AttendanceService } from '../../../core/services/attendance.service';
import { Employee, EmployeeService } from '../../../core/services/employee.service';

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
    MatIconModule
  ],
  templateUrl: './check-in.component.html',
  styleUrl: './check-in.component.scss'
})
export class CheckInComponent implements OnInit {

  private readonly attendanceService = inject(AttendanceService);
  private readonly employeeService = inject(EmployeeService);
  private readonly fb = inject(FormBuilder);

  employees: Employee[] = [];

  todayAttendance: Attendance[] = [];

  form = this.fb.group({
    employeeId: [null as number | null, Validators.required]
  });

  ngOnInit(): void {
    this.loadEmployees();
    this.loadTodayAttendance();
  }

  loadEmployees(): void {
    this.employeeService.getActive().subscribe({
      next: (res) => {
        this.employees = res;
      },
      error: (err) => console.error(err)
    });
  }

  loadTodayAttendance(): void {
    this.attendanceService.getTodayAttendance().subscribe({
      next: (res) => {
        this.todayAttendance = res;
      },
      error: (err) => console.error(err)
    });
  }

  checkIn(): void {

    if (this.form.invalid) {
      return;
    }

    this.attendanceService.checkIn({
      employeeId: this.form.value.employeeId!
    }).subscribe({
      next: () => {
        alert('Employee checked in successfully.');
        this.loadTodayAttendance();
      },
      error: (err) => console.error(err)
    });

  }

  checkOut(): void {

    if (this.form.invalid) {
      return;
    }

    this.attendanceService.checkOut({
      employeeId: this.form.value.employeeId!
    }).subscribe({
      next: () => {
        alert('Employee checked out successfully.');
        this.loadTodayAttendance();
      },
      error: (err) => console.error(err)
    });

  }

  getSelectedAttendance(): Attendance | null {

    const employeeId = this.form.value.employeeId;

    if (!employeeId) {
      return null;
    }

    return (
      this.todayAttendance.find(x => x.employeeId === employeeId) ?? null
    );
  }

}