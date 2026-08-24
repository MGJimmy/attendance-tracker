import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { AddEmployeeDTO, EditEmployeeDTO, EmployeeService } from '../../../core/services/employee.service';
import { UserDto, UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-add-edit-employee',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './add-edit-employee.component.html',
  styleUrl: './add-edit-employee.component.scss'
})
export class AddEditEmployeeComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly employeeService = inject(EmployeeService);
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  employeeId = 0;
  isEditMode = false;
  users: UserDto[] = [];
  errorMessage = '';

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    salaryPerHour: [null as number | null, [Validators.required, Validators.min(0.01)]],
    userId: ['', Validators.required]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.employeeId = +id;
      this.isEditMode = true;
      this.loadEmployee();
    }

    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAvailable(this.isEditMode ? this.employeeId : undefined).subscribe({
      next: users => this.users = users,
      error: error => console.error(error)
    });
  }

  loadEmployee(): void {
    this.employeeService.getById(this.employeeId).subscribe({
      next: employee => {
        this.form.patchValue({
          name: employee.name,
          salaryPerHour: employee.salaryPerHour,
          userId: employee.userId
        });
      },
      error: error => console.error(error)
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage = '';

    if (this.isEditMode) {
      this.update();
    } else {
      this.add();
    }
  }

  private add(): void {
    const payload: AddEmployeeDTO = {
      name: this.form.value.name!,
      salaryPerHour: this.form.value.salaryPerHour!,
      userId: this.form.value.userId!
    };

    this.employeeService.add(payload).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private update(): void {
    const payload: EditEmployeeDTO = {
      id: this.employeeId,
      name: this.form.value.name!,
      salaryPerHour: this.form.value.salaryPerHour!,
      userId: this.form.value.userId!
    };

    this.employeeService.edit(payload).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private setError(error: HttpErrorResponse): void {
    this.errorMessage = typeof error.error === 'string'
      ? error.error
      : 'Could not save employee.';
  }

}
