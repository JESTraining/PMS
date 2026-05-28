import { Component, inject, PLATFORM_ID, signal } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

const SIDEBAR_COLLAPSED_KEY = 'pms_sidebar_collapsed';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss'
})
export class AppLayoutComponent {
  private readonly platformId = inject(PLATFORM_ID);
  userName = '';
  user = '';
  readonly sidebarCollapsed = signal(false);

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.userName = localStorage.getItem('pms_user_name') || '';
      this.user = localStorage.getItem('pms_user') || '';
      if (localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === '1') {
        this.sidebarCollapsed.set(true);
      }
    }
  }

  get userInitial(): string {
    const n = (this.userName || this.user || '?').trim();
    if (!n) {
      return '?';
    }
    const parts = n.split(/\s+/).filter(Boolean);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return n.slice(0, 2).toUpperCase();
  }

  toggleSidebar(): void {
    this.sidebarCollapsed.update((v) => !v);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(SIDEBAR_COLLAPSED_KEY, this.sidebarCollapsed() ? '1' : '0');
    }
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
