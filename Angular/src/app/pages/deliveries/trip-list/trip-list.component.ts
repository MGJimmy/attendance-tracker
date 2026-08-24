import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';

import { Courier, CourierService } from '../../../core/services/courier.service';
import { Trip, TripService } from '../../../core/services/trip.service';
import { DATE_FORMAT } from '../../../shared/date-formats';
import { EgyptDatePipe } from '../../../shared/egypt-date.pipe';

@Component({
  selector: 'app-trip-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    EgyptDatePipe
  ],
  templateUrl: './trip-list.component.html',
  styleUrl: './trip-list.component.scss'
})
export class TripListComponent implements OnInit {

  private readonly tripService = inject(TripService);
  private readonly courierService = inject(CourierService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  readonly dateFormat = DATE_FORMAT;

  couriers: Courier[] = [];
  trips: Trip[] = [];

  displayedColumns: string[] = [
    'courierName',
    'destinationName',
    'cost',
    'tripDate',
    'actions'
  ];

  form = this.fb.group({
    courierId: [null as number | null],
    from: [null as Date | null],
    to: [null as Date | null]
  });

  ngOnInit(): void {
    this.courierService.getAll().subscribe({
      next: couriers => this.couriers = couriers,
      error: error => console.error(error)
    });
    this.search();
  }

  search(): void {
    const from = this.form.value.from;
    const to = this.form.value.to;

    this.tripService.getAll({
      courierId: this.form.value.courierId ?? undefined,
      from: from ? this.toDateOnly(from) : undefined,
      to: to ? this.toDateOnly(to) : undefined
    }).subscribe({
      next: trips => this.trips = trips,
      error: error => console.error(error)
    });
  }

  clear(): void {
    this.form.reset();
    this.search();
  }

  add(): void {
    this.router.navigate(['/trips/add']);
  }

  edit(id: number): void {
    this.router.navigate(['/trips/edit', id]);
  }

  viewCourier(id: number): void {
    this.router.navigate(['/couriers', id]);
  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

}
