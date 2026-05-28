import { Component, OnInit, PLATFORM_ID, inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

const REMEMBER_USER_KEY = 'pms_login_remember_username';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private readonly platformId = inject(PLATFORM_ID);

  credentials = { username: '', password: '' };
  rememberMe = false;
  passwordVisible = false;
  selectedRole: 'admin' | 'doctor' | 'patient' | null = null;
  isSubmitting = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }
    const saved = localStorage.getItem(REMEMBER_USER_KEY);
    if (saved) {
      this.credentials.username = saved;
      this.rememberMe = true;
    }
  }

  selectRole(role: 'admin' | 'doctor' | 'patient'): void {
    this.selectedRole = role;
    const presets: Record<typeof role, string> = {
      admin: 'admin',
      doctor: 'doctor',
      patient: 'patient'
    };
    this.credentials.username = presets[role];
  }

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  noopClick(event: Event): void {
    event.preventDefault();
  }

  async onLogin(): Promise<void> {
    if (this.isSubmitting) {
      return;
    }
    this.isSubmitting = true;
    try {
      const loginProcess = await this.authService.login(this.credentials);
      if (loginProcess) {
        if (this.rememberMe) {
          localStorage.setItem(REMEMBER_USER_KEY, this.credentials.username);
        } else {
          localStorage.removeItem(REMEMBER_USER_KEY);
        }
        this.router.navigate(['/dashboard']);
      }
    } finally {
      this.isSubmitting = false;
    }
  }
}
