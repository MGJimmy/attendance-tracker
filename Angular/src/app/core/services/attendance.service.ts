import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Attendance {
  employeeId: number;
  name: string;
  attendanceDate: string;
  checkInTime: string | null;
  checkOutTime: string | null;
  workingHours: string | null;
}

export interface CheckInDTO {
  employeeId: number;
}

export interface CheckOutDTO {
  employeeId: number;
}

export interface AttendanceHistoryDTO {
  employeeId?: number;
  from?: string;
  to?: string;
}

export interface Dashboard {
  totalEmployees: number;
  activeEmployees: number;
  checkedIn: number;
  checkedOut: number;
  absent: number;
}

export interface Attendance {
  employeeId: number;
  name: string;
  attendanceDate: string;
  checkInTime: string | null;
  checkOutTime: string | null;
  workingHours: string | null;
}

export interface AttendanceReportDTO {
  employeeId?: number;
  from?: string;
  to?: string;
}

export interface AttendanceReportRow {
  employeeId: number;
  employeeName: string;
  attendanceDate: string;

  checkInTime: string | null;
  checkOutTime: string | null;

  workingHours: string | null;
}

export interface AttendanceReportResult {
  workingDays: number;
  completedDays: number;
  openDays: number;

  totalWorkingHours: string;
  averageWorkingHours: string;

  earliestCheckIn: string | null;
  latestCheckOut: string | null;

  longestShift: string | null;
  shortestShift: string | null;

  records: AttendanceReportRow[];
}

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {

  private readonly http = inject(HttpClient);

  private readonly api = `${environment.apiUrl}/employee`;

  //#region Check In / Check Out

  checkIn(payload: CheckInDTO): Observable<any> {
    return this.http.post(`${this.api}/check-in`, payload);
  }

  checkOut(payload: CheckOutDTO): Observable<any> {
    return this.http.post(`${this.api}/check-out`, payload);
  }

  //#endregion

  //#region Attendance

  getTodayAttendance(): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(`${this.api}/today`);
  }

  getEmployeeHistory(employeeId: number): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(
      `${this.api}/employee/${employeeId}`
    );
  }

  getHistory(payload: AttendanceHistoryDTO): Observable<Attendance[]> {
    return this.http.post<Attendance[]>(
      `${this.api}/history`,
      payload
    );
  }

  //#endregion

  //#region Report
  getReport(
    payload: AttendanceReportDTO
  ): Observable<AttendanceReportResult> {
    return this.http.post<AttendanceReportResult>(
      `${this.api}/report`,
      payload
    );
  }

  //#endregion

  
  //#region Dashboard

  getDashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>(
      `${this.api}/dashboard`
    );
  }

  getCurrentlyWorking(): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(
      `${this.api}/currently-working`
    );
  }

  getTodayAbsent(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.api}/today-absent`
    );
  }

  //#endregion
}