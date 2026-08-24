import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { Destination, DestinationService } from '../../../core/services/destination.service';

@Component({
  selector: 'app-destination-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule
  ],
  templateUrl: './destination-list.component.html',
  styleUrl: './destination-list.component.scss'
})
export class DestinationListComponent implements OnInit {

  private readonly destinationService = inject(DestinationService);
  private readonly router = inject(Router);

  destinations: Destination[] = [];

  displayedColumns: string[] = [
    'name',
    'cost',
    'details',
    'status',
    'actions'
  ];

  ngOnInit(): void {
    this.loadDestinations();
  }

  loadDestinations(): void {
    this.destinationService.getAll().subscribe({
      next: response => this.destinations = response,
      error: error => console.error(error)
    });
  }

  edit(id: number): void {
    this.router.navigate(['/destinations/edit', id]);
  }

  add(): void {
    this.router.navigate(['/destinations/add']);
  }

  activate(id: number): void {
    this.destinationService.activate(id).subscribe({
      next: () => this.loadDestinations(),
      error: error => console.error(error)
    });
  }

  deactivate(id: number): void {
    this.destinationService.deactivate(id).subscribe({
      next: () => this.loadDestinations(),
      error: error => console.error(error)
    });
  }

}
