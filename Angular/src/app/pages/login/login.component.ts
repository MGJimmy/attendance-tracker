import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService, LoginResult } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  hidePassword = true;
  loading = false;
  errorMessage = '';

  readonly heroImage =
    'https://images.unsplash.com/photo-1587854692152-cbe660dbde88?auto=format&fit=crop&w=1800&q=80';
  readonly heroCredit = 'https://unsplash.com/photos/white-plastic-pack-on-white-table-cbe660dbde88';

  readonly pharmacistImage =
    'https://images.pexels.com/photos/3873146/pexels-photo-3873146.jpeg?auto=compress&cs=tinysrgb&w=800';
  readonly pharmacistCredit = 'https://www.pexels.com/photo/a-pharmacist-holding-a-medicine-container-3873146/';

  readonly pillsImage =
    'https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?auto=format&fit=crop&w=800&q=80';
  readonly pillsCredit = 'https://unsplash.com/photos/white-and-blue-medication-pill-blister-pack-jwWtZrm67VI';

  readonly form = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid || this.loading) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const { username, password } = this.form.getRawValue();

    this.auth.login(username, password).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        const body = err.error as LoginResult | undefined;
        this.errorMessage =
          body?.loginResponse?.message ||
          'Invalid username or password. Please try again.';
      }
    });
  }
}
