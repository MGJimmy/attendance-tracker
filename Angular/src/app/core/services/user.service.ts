import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserDto {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  roles: string[];
  isActive: boolean;
}

export interface CreateUserPayload {
  username: string;
  password: string;
  role: string;
}

export interface CreateUserResponse {
  success: boolean;
  message: string;
  userId: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl + '/users';

  create(payload: CreateUserPayload): Observable<CreateUserResponse> {
    return this.http.post<CreateUserResponse>(this.api, payload);
  }

  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.api);
  }

  getAvailable(employeeId?: number): Observable<UserDto[]> {
    let params = new HttpParams();
    if (employeeId) {
      params = params.set('employeeId', employeeId);
    }

    return this.http.get<UserDto[]>(`${this.api}/available`, { params });
  }

  activate(id: string): Observable<any> {
    return this.http.post(`${this.api}/activate/${id}`, {});
  }

  deactivate(id: string): Observable<any> {
    return this.http.post(`${this.api}/deactivate/${id}`, {});
  }
}
