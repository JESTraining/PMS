import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class BillingService {
  private apiUrl = 'http://localhost:5231/api/invoices';
  constructor(private http: HttpClient) { }

  getPendingInvoices(): Observable<PendingInvoiceInterface[]> {
    return this.http.get<PendingInvoiceInterface[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('pms_token')}`
      }
    });
  }

  saveInvoiceBilling(billing: PendingInvoiceInterface): Observable<any> {
    var billingUri = `${this.apiUrl}/createBilling`;
    return this.http.post<any>(billingUri, billing, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('pms_token')}`
      }
    });
  }

}
