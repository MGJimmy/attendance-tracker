import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import {
  AddDestinationDTO,
  DestinationService,
  EditDestinationDTO
} from '../../../core/services/destination.service';

@Component({
  selector: 'app-add-edit-destination',
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
  templateUrl: './add-edit-destination.component.html',
  styleUrl: './add-edit-destination.component.scss'
})
export class AddEditDestinationComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly destinationService = inject(DestinationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  destinationId = 0;
  isEditMode = false;
  errorMessage = '';

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    cost: [null as number | null, [Validators.required, Validators.min(0)]],
    details: ['', Validators.maxLength(500)]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.destinationId = +id;
      this.isEditMode = true;
      this.loadDestination();
    }
  }

  loadDestination(): void {
    this.destinationService.getById(this.destinationId).subscribe({
      next: destination => {
        this.form.patchValue({
          name: destination.name,
          cost: destination.cost,
          details: destination.details
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
    const payload: AddDestinationDTO = {
      name: this.form.value.name!,
      cost: this.form.value.cost!,
      details: this.form.value.details ?? ''
    };

    this.destinationService.add(payload).subscribe({
      next: () => this.router.navigate(['/destinations']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private update(): void {
    const payload: EditDestinationDTO = {
      id: this.destinationId,
      name: this.form.value.name!,
      cost: this.form.value.cost!,
      details: this.form.value.details ?? ''
    };

    this.destinationService.edit(payload).subscribe({
      next: () => this.router.navigate(['/destinations']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private setError(error: HttpErrorResponse): void {
    this.errorMessage = typeof error.error === 'string'
      ? error.error
      : 'Could not save destination.';
  }

}
