import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { HttpErrorResponse } from '@angular/common/http';

import { Attendance, AttendanceService } from '../../../core/services/attendance.service';
import { toEgyptDateTimeInput } from '../../../shared/date-formats';

@Component({
  selector: 'app-edit-attendance-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './edit-attendance-dialog.component.html',
  styleUrl: './edit-attendance-dialog.component.scss'
})
export class EditAttendanceDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly attendanceService = inject(AttendanceService);
  private readonly dialogRef = inject(MatDialogRef<EditAttendanceDialogComponent>);
  readonly data = inject<Attendance>(MAT_DIALOG_DATA);

  errorMessage = '';
  saving = false;

  form = this.fb.group({
    checkInTime: [toEgyptDateTimeInput(this.data.checkInTime), Validators.required],
    checkOutTime: [this.data.checkOutTime ? toEgyptDateTimeInput(this.data.checkOutTime) : '']
  });

  save(): void {
    if (this.form.invalid || this.saving) {
      this.form.markAllAsTouched();
      return;
    }

    const checkIn = this.form.value.checkInTime!;
    const checkOut = this.form.value.checkOutTime;

    if (checkOut && checkOut <= checkIn) {
      this.errorMessage = 'Check-out time must be after check-in time.';
      return;
    }

    if (!this.data.id) {
      this.errorMessage = 'Attendance record id is missing.';
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    this.attendanceService.editAttendance(this.data.id, {
      checkInTime: checkIn,
      checkOutTime: checkOut ? checkOut : null
    }).subscribe({
      next: () => this.dialogRef.close(true),
      error: (err: HttpErrorResponse) => {
        this.saving = false;
        const body = err.error;
        this.errorMessage = typeof body === 'string'
          ? body
          : body?.message || 'Could not update attendance.';
      }
    });
  }
}
