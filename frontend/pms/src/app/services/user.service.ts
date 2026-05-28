import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { getPmsToken } from '../utils/storage.util';

@Injectable({
  providedIn: 'root'
})

export class UserService {
  private apiUrl = 'http://localhost:5231/api/users';
  constructor(private http: HttpClient) { }

  getUsers(): Observable<UserInterface[]> {
    return this.http.get<UserInterface[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }

  createUser(user: UserInterface): Observable<any> {
    return this.http.post<boolean>(this.apiUrl, user, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }
  updateUser(user: UserInterface) {
    return this.http.put(this.apiUrl, user, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }

  deleteUser(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }
}
