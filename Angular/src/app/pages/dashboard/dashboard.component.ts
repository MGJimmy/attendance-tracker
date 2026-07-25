import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';

import {
  Attendance,
  AttendanceService,
  Dashboard
} from '../../core/services/attendance.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  private readonly attendanceService = inject(AttendanceService);

  dashboard: Dashboard | null = null;

  todayAttendance: Attendance[] = [];

  displayedColumns: string[] = [
    'name',
    'checkInTime',
    'checkOutTime',
    'workingHours'
  ];

  ngOnInit(): void {
    this.loadDashboard();
    this.loadTodayAttendance();
  }

  loadDashboard(): void {
    this.attendanceService.getDashboard().subscribe({
      next: (response) => {
        this.dashboard = response;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  loadTodayAttendance(): void {
    this.attendanceService.getTodayAttendance().subscribe({
      next: (response) => {
        this.todayAttendance = response;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

}