import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';

import { CourierService, CourierSummary } from '../../../core/services/courier.service';
import { DATE_FORMAT } from '../../../shared/date-formats';
import { EgyptDatePipe } from '../../../shared/egypt-date.pipe';

@Component({
  selector: 'app-courier-details',
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
    MatTableModule,
    EgyptDatePipe
  ],
  templateUrl: './courier-details.component.html',
  styleUrl: './courier-details.component.scss'
})
export class CourierDetailsComponent implements OnInit {

  private readonly courierService = inject(CourierService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  readonly dateFormat = DATE_FORMAT;

  courierId = 0;
  summary: CourierSummary | null = null;

  displayedColumns: string[] = [
    'destinationName',
    'tripDate',
    'cost'
  ];

  form = this.fb.group({
    from: [null as Date | null],
    to: [null as Date | null]
  });

  ngOnInit(): void {
    this.courierId = Number(this.route.snapshot.paramMap.get('id'));
    this.search();
  }

  search(): void {
    const from = this.form.value.from;
    const to = this.form.value.to;

    this.courierService.getSummary(this.courierId, {
      from: from ? this.toDateOnly(from) : undefined,
      to: to ? this.toDateOnly(to) : undefined
    }).subscribe({
      next: summary => this.summary = summary,
      error: error => console.error(error)
    });
  }

  clear(): void {
    this.form.reset();
    this.search();
  }

  back(): void {
    this.router.navigate(['/couriers']);
  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

}
