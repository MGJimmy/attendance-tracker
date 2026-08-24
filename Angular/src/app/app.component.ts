import { Component, ViewChild, inject } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { filter } from 'rxjs/operators';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';

import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatExpansionModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);

  @ViewChild('drawer') drawer?: MatSidenav;

  isLoginPage = this.router.url.startsWith('/login');
  attendanceOpen = this.isAttendanceUrl(this.router.url);
  deliveriesOpen = this.isDeliveriesUrl(this.router.url);

  constructor() {
    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe(event => {
        this.isLoginPage = event.urlAfterRedirects.startsWith('/login');
        if (this.isAttendanceUrl(event.urlAfterRedirects)) {
          this.attendanceOpen = true;
        }
        if (this.isDeliveriesUrl(event.urlAfterRedirects)) {
          this.deliveriesOpen = true;
        }
      });
  }

  private isAttendanceUrl(url: string): boolean {
    return url.startsWith('/employees')
      || url.startsWith('/attendance');
  }

  private isDeliveriesUrl(url: string): boolean {
    return url.startsWith('/couriers')
      || url.startsWith('/destinations')
      || url.startsWith('/trips');
  }

  toggleMenu(): void {
    this.drawer?.toggle();
  }

  logout(): void {
    this.auth.logout();
  }
}
