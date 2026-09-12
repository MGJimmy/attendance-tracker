import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { UserDto, UserService } from '../../../core/services/user.service';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatDialogModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

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

  editPassword(user: UserDto): void {
    this.dialog.open(ChangePasswordDialogComponent, {
      width: '480px',
      data: user
    });
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
