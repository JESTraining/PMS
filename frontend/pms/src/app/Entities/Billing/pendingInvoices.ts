interface PendingInvoiceInterface {
  encounterId: number;
  patientName: string;
  encounterDate: string;
  invoiceDetails: PendingInvoiceDetailInterface[];
}
