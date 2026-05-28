import { Injectable } from '@angular/core';
import { getPmsToken } from '../../utils/storage.util';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient) {}

  async login(credentials: any): Promise<boolean> {
    try {
      const requestBody = {
        username: credentials.username,
        password: credentials.password
      };

      const response = await fetch(
        'http://localhost:5231/api/login',
        {
          method: 'POST',
          headers: {'Content-Type': 'application/json' },
          body: JSON.stringify(requestBody),
        });

      if (!response.ok) {
        console.log(response);
        alert('Login failed: ' + response.statusText);
        return false;
      }

      const data = await response.json();
      const token = data.token;
      const decodedToken = this.decodeToken(token);

      localStorage.setItem('pms_token', token);
      localStorage.setItem('pms_user', decodedToken.sub);
      localStorage.setItem('pms_user_name', decodedToken.Name);

      return response.ok;
    }
    catch (error: any) {
      alert('Login failed: ' + error.message);
      return false;
    }
  }

  decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }


  logout(): void {
    localStorage.removeItem('pms_token');
  }

  isAuthenticated(): boolean {
    return !!getPmsToken();
  }
}
