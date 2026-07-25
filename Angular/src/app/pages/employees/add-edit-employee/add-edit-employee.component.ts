import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AddEmployeeDTO, EditEmployeeDTO, EmployeeService } from '../../../core/services/employee.service';



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
    MatButtonModule
  ],
  templateUrl: './add-edit-employee.component.html',
  styleUrl: './add-edit-employee.component.scss'
})
export class AddEditEmployeeComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly employeeService = inject(EmployeeService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  employeeId = 0;

  isEditMode = false;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    salary: [null as number | null, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {

    const id = this.route.snapshot.paramMap.get('id');

    if (!id)
      return;

    this.employeeId = +id;
    this.isEditMode = true;

    this.loadEmployee();
  }

  loadEmployee(): void {

    this.employeeService.getById(this.employeeId).subscribe({
      next: employee => {

        this.form.patchValue({
          name: employee.name,
          salary: employee.salary
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

    if (this.isEditMode) {
      this.update();
    }
    else {
      this.add();
    }

  }

  private add(): void {

    const payload: AddEmployeeDTO = {
      name: this.form.value.name!,
      salary: this.form.value.salary!
    };

    this.employeeService.add(payload).subscribe({
      next: () => {
        this.router.navigate(['/employees']);
      },
      error: error => console.error(error)
    });

  }

  private update(): void {

    const payload: EditEmployeeDTO = {
      id: this.employeeId,
      name: this.form.value.name!,
      salary: this.form.value.salary!
    };

    this.employeeService.edit(payload).subscribe({
      next: () => {
        this.router.navigate(['/employees']);
      },
      error: error => console.error(error)
    });

  }

}