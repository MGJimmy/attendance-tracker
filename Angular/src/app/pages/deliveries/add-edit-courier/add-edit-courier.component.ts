import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { AddCourierDTO, CourierService, EditCourierDTO } from '../../../core/services/courier.service';

@Component({
  selector: 'app-add-edit-courier',
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
  templateUrl: './add-edit-courier.component.html',
  styleUrl: './add-edit-courier.component.scss'
})
export class AddEditCourierComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly courierService = inject(CourierService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  courierId = 0;
  isEditMode = false;
  errorMessage = '';

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    mobileNumber: ['', [Validators.required, Validators.maxLength(30)]]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.courierId = +id;
      this.isEditMode = true;
      this.loadCourier();
    }
  }

  loadCourier(): void {
    this.courierService.getById(this.courierId).subscribe({
      next: courier => {
        this.form.patchValue({
          name: courier.name,
          mobileNumber: courier.mobileNumber
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
    const payload: AddCourierDTO = {
      name: this.form.value.name!,
      mobileNumber: this.form.value.mobileNumber!
    };

    this.courierService.add(payload).subscribe({
      next: () => this.router.navigate(['/couriers']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private update(): void {
    const payload: EditCourierDTO = {
      id: this.courierId,
      name: this.form.value.name!,
      mobileNumber: this.form.value.mobileNumber!
    };

    this.courierService.edit(payload).subscribe({
      next: () => this.router.navigate(['/couriers']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private setError(error: HttpErrorResponse): void {
    this.errorMessage = typeof error.error === 'string'
      ? error.error
      : 'Could not save courier.';
  }

}
