import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';

import { Courier, CourierService } from '../../../core/services/courier.service';

@Component({
  selector: 'app-courier-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
    MatTableModule
  ],
  templateUrl: './courier-list.component.html',
  styleUrl: './courier-list.component.scss'
})
export class CourierListComponent implements OnInit {

  private readonly courierService = inject(CourierService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  couriers: Courier[] = [];
  filtered: Courier[] = [];

  filterForm = this.fb.group({
    name: [null as string | null]
  });

  displayedColumns: string[] = [
    'name',
    'mobileNumber',
    'status',
    'actions'
  ];

  ngOnInit(): void {
    this.loadCouriers();
    this.filterForm.controls.name.valueChanges.subscribe(() => this.applyFilter());
  }

  loadCouriers(): void {
    this.courierService.getAll().subscribe({
      next: response => {
        this.couriers = response;
        this.applyFilter();
      },
      error: error => console.error(error)
    });
  }

  applyFilter(): void {
    const name = this.filterForm.value.name;
    this.filtered = name
      ? this.couriers.filter(courier => courier.name === name)
      : this.couriers;
  }

  view(id: number): void {
    this.router.navigate(['/couriers', id]);
  }

  edit(id: number): void {
    this.router.navigate(['/couriers/edit', id]);
  }

  add(): void {
    this.router.navigate(['/couriers/add']);
  }

  activate(id: number): void {
    this.courierService.activate(id).subscribe({
      next: () => this.loadCouriers(),
      error: error => console.error(error)
    });
  }

  deactivate(id: number): void {
    this.courierService.deactivate(id).subscribe({
      next: () => this.loadCouriers(),
      error: error => console.error(error)
    });
  }

}
