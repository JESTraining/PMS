import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DoctorService } from '../../services/doctor/doctor.service';
import { DailyScheduleModalComponent } from '../../components/daily-schedule-modal.component';

@Component({
  selector: 'app-schedule',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './schedule.component.html',
  styleUrl: './schedule.component.css'
})
export class ScheduleComponent implements OnInit {
  doctorService = inject(DoctorService);
  dialog = inject(MatDialog);

  searchTerm = signal<string>('');
  doctorsList = signal<any[]>([]);

  ngOnInit() {
    this.onSearch(); // Carga inicial
  }

  onSearch() {
    this.doctorService.searchDoctors(this.searchTerm()).subscribe({
      next: (data) => this.doctorsList.set(data),
      error: (err) => console.error(err)
    });
  }

  openDailySchedule(doctor: any) {
    this.dialog.open(DailyScheduleModalComponent, {
      width: '800px',
      height: '80vh',
      data: { doctorId: doctor.id, doctorName: doctor.name }
    });
  }
}
