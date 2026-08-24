import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { Courier, CourierService } from '../../../core/services/courier.service';
import { Destination, DestinationService } from '../../../core/services/destination.service';
import { AddTripDTO, EditTripDTO, TripService } from '../../../core/services/trip.service';
import { toEgyptDateTimeInput } from '../../../shared/date-formats';

@Component({
  selector: 'app-add-edit-trip',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './add-edit-trip.component.html',
  styleUrl: './add-edit-trip.component.scss'
})
export class AddEditTripComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly tripService = inject(TripService);
  private readonly courierService = inject(CourierService);
  private readonly destinationService = inject(DestinationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  tripId = 0;
  isEditMode = false;
  errorMessage = '';
  couriers: Courier[] = [];
  destinations: Destination[] = [];

  form = this.fb.group({
    courierId: [null as number | null, Validators.required],
    destinationId: [null as number | null, Validators.required],
    tripDate: [this.egyptToday(), Validators.required],
    cost: [null as number | null, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.tripId = +id;
      this.isEditMode = true;
    }

    this.form.controls.destinationId.valueChanges.subscribe(destinationId => {
      const destination = this.destinations.find(item => item.id === destinationId);
      if (destination) {
        this.form.controls.cost.setValue(destination.cost);
      }
    });

    this.courierService.getAll().subscribe({
      next: couriers => {
        this.couriers = this.isEditMode
          ? couriers
          : couriers.filter(courier => courier.isActive);
      },
      error: error => console.error(error)
    });

    this.destinationService.getAll().subscribe({
      next: destinations => {
        this.destinations = this.isEditMode
          ? destinations
          : destinations.filter(destination => destination.isActive);

        if (this.isEditMode) {
          this.loadTrip();
        }
      },
      error: error => console.error(error)
    });
  }

  get courierOptions(): Courier[] {
    if (!this.isEditMode) {
      return this.couriers.filter(courier => courier.isActive);
    }

    const selectedId = this.form.value.courierId;
    return this.couriers.filter(courier => courier.isActive || courier.id === selectedId);
  }

  get destinationOptions(): Destination[] {
    if (!this.isEditMode) {
      return this.destinations.filter(destination => destination.isActive);
    }

    const selectedId = this.form.value.destinationId;
    return this.destinations.filter(destination => destination.isActive || destination.id === selectedId);
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

  private loadTrip(): void {
    this.tripService.getById(this.tripId).subscribe({
      next: trip => {
        this.form.patchValue({
          courierId: trip.courierId,
          destinationId: trip.destinationId,
          tripDate: this.parseDateOnly(trip.tripDate),
          cost: trip.cost
        }, { emitEvent: false });
      },
      error: error => console.error(error)
    });
  }

  private add(): void {
    const payload: AddTripDTO = {
      courierId: this.form.value.courierId!,
      destinationId: this.form.value.destinationId!,
      cost: this.form.value.cost!,
      tripDate: this.toDateOnly(this.form.value.tripDate!)
    };

    this.tripService.add(payload).subscribe({
      next: () => this.router.navigate(['/trips']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private update(): void {
    const payload: EditTripDTO = {
      id: this.tripId,
      courierId: this.form.value.courierId!,
      destinationId: this.form.value.destinationId!,
      cost: this.form.value.cost!,
      tripDate: this.toDateOnly(this.form.value.tripDate!)
    };

    this.tripService.edit(payload).subscribe({
      next: () => this.router.navigate(['/trips']),
      error: (error: HttpErrorResponse) => this.setError(error)
    });
  }

  private egyptToday(): Date {
    const [datePart] = toEgyptDateTimeInput().split('T');
    return this.parseDateOnly(datePart);
  }

  private parseDateOnly(value: string): Date {
    const [year, month, day] = value.split('-').map(Number);
    return new Date(year, month - 1, day);
  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private setError(error: HttpErrorResponse): void {
    this.errorMessage = typeof error.error === 'string'
      ? error.error
      : 'Could not save trip.';
  }

}
