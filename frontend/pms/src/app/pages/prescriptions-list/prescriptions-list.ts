import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrescriptionService } from '../../services/prescription/prescription.service';
import { Prescription } from '../../models/prescription.model';
import { PrescriptionModalComponent } from '../prescription-modal/prescription-modal.component';

@Component({
  selector: 'app-prescriptions-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PrescriptionModalComponent],
  templateUrl: './prescriptions-list.html'
})
export class PrescriptionsList implements OnInit {

  prescriptions = signal<Prescription[]>([]);
  showModal = signal(false);
  selectedPrescription = signal<Prescription | null>(null);

  // 🔍 FILTERS
  searchTerm = '';
  selectedDoctor = '';
  selectedDate = '';

  doctors: string[] = [];

  // 📄 DATA
  filteredData: Prescription[] = [];
  paginatedData: Prescription[] = [];

  // PAGINATION
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;

  constructor(private service: PrescriptionService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.service.getAll().subscribe(data => {
      const result = data || [];
      this.prescriptions.set(result);

      this.doctors = [
        ...new Set(
          result
            .map(p => p.doctor?.name)
            .filter((name): name is string => !!name)
        )
      ];

      this.filteredData = result;
      this.updatePagination();
    });
  }

  // 🔍 FILTER LOGIC
  applyFilters() {
    let data = this.prescriptions();

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();

      data = data.filter(p =>
        p.patient?.firstName?.toLowerCase().includes(term) ||
        p.patient?.lastName?.toLowerCase().includes(term) ||
        p.doctor?.name?.toLowerCase().includes(term) ||
        p.medications?.some(m =>
          m.medication?.name?.toLowerCase().includes(term)
        )
      );
    }

    if (this.selectedDoctor) {
      data = data.filter(p => p.doctor?.name === this.selectedDoctor);
    }

    if (this.selectedDate) {
      data = data.filter(p => p.issueDate?.startsWith(this.selectedDate));
    }

    this.filteredData = data;
    this.currentPage = 1;

    this.updatePagination();
  }

  // 📄 PAGINATION
  updatePagination() {
    this.totalPages = Math.ceil(this.filteredData.length / this.pageSize) || 1;

    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;

    this.paginatedData = this.filteredData.slice(start, end);
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  openCreateModal() {
    this.selectedPrescription.set(null);
    this.showModal.set(true);
  }

  openEditModal(p: Prescription) {
    this.selectedPrescription.set(p);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  delete(id: number) {
    if (!confirm('Delete this prescription?')) return;
    this.service.delete(id).subscribe(() => this.load());
  }
}