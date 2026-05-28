import { Component, OnInit, Signal, signal } from '@angular/core';
import { BillingService } from '../../services/billing/billing.service';
import { BillingReviewComponent } from '../billing-review.component/billing-review.component';

@Component({
  selector: 'app-billing.component',
  imports: [BillingReviewComponent],
  templateUrl: './billing.component.html',
  styleUrl: './billing.component.css',
})
export class BillingComponent implements OnInit {
  pendingBillingData = signal<PendingInvoiceInterface[]>([]);
  currentPendingBilling: PendingInvoiceInterface = {
    encounterId: 0,
    patientName: '',
    encounterDate: '',
    invoiceDetails: []
  };
  isEditing = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  constructor(private billingService: BillingService) {}

  ngOnInit() {
    this.getPendingInvoices();
  }

  getPendingInvoices() {
    this.billingService.getPendingInvoices().subscribe((invoices) => {
      this.pendingBillingData.set(invoices);
      console.log(invoices);
    });
  }

  openEditBilling(billing: PendingInvoiceInterface): void {
    this.isEditing.set(true);
    this.currentPendingBilling = { ...billing };
  }

  savePendingBilling(updatedBilling: PendingInvoiceInterface): void {
    this.isSaving.set(true);
    this.billingService.saveInvoiceBilling(updatedBilling).subscribe({
      next: () => {
        this.getPendingInvoices();
        this.closeModal();
      },
      error: (error) => {
        console.error('Error saving billing:', error);
        alert('Failed to save billing. Please try again.');
        this.isSaving.set(false);
      }
    });
    console.log('Saving billing:', updatedBilling);
    setTimeout(() => {
      this.isSaving.set(false);
      this.closeModal();
    }, 1000);
  }

  closeModal(): void {
    this.isEditing.set(false);
    this.currentPendingBilling = {
      encounterId: 0,
      patientName: '',
      encounterDate: '',
      invoiceDetails: []
    };
  }
}
