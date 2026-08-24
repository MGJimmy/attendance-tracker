import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { UserDto, UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  users: UserDto[] = [];

  displayedColumns: string[] = [
    'userName',
    'role',
    'status',
    'actions'
  ];

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: response => {
        this.users = response;
      },
      error: error => console.error(error)
    });
  }

  add(): void {
    this.router.navigate(['/users/create']);
  }

  roleLabel(user: UserDto): string {
    return user.roles?.length ? user.roles.join(', ') : '-';
  }

  activate(id: string): void {
    this.userService.activate(id).subscribe({
      next: () => this.loadUsers(),
      error: error => console.error(error)
    });
  }

  deactivate(id: string): void {
    this.userService.deactivate(id).subscribe({
      next: () => this.loadUsers(),
      error: error => console.error(error)
    });
  }
}
