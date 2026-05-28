import { Component, inject, effect, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ScheduleService } from '../services/schedule/schedule.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { getPmsToken } from '../utils/storage.util';

import { EncounterService } from '../services/encounter/encounter.service';
import { EncounterModal } from '../pages/encounter-modal/encounter-modal';

@Component({
  selector: 'app-daily-schedule-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  template: `
    <div class="flex flex-col h-full bg-white rounded-lg">
      <div class="p-4 border-b border-gray-200 flex justify-between items-center bg-gray-50 rounded-t-lg">
        <h2 class="text-xl font-bold text-gray-800" mat-dialog-title>
          Schedule for {{ data.doctorName }}
        </h2>
        <input 
          type="date" 
          [ngModel]="scheduleService.selectedDate() | date:'yyyy-MM-dd'"
          (ngModelChange)="onDateChange($event)"
          class="border border-gray-300 rounded p-2 text-sm focus:ring-blue-500 focus:border-blue-500"
        >
      </div>

      <div class="flex border-b border-gray-200 px-4 bg-gray-50">
        <button (click)="activeTab.set('overview')" 
                [ngClass]="activeTab() === 'overview' ? 'border-blue-600 text-blue-600 border-b-2' : 'border-transparent text-gray-500 hover:text-gray-700'"
                class="px-4 py-3 font-medium text-sm transition-colors duration-200 focus:outline-none">
          Grid Schedule
        </button>
        <button (click)="activeTab.set('new')" 
                [ngClass]="activeTab() === 'new' ? 'border-blue-600 text-blue-600 border-b-2' : 'border-transparent text-gray-500 hover:text-gray-700'"
                class="px-4 py-3 font-medium text-sm transition-colors duration-200 focus:outline-none">
          + Schedule Appointment
        </button>
      </div>

      <div mat-dialog-content class="flex-1 p-4 overflow-y-auto relative min-h-[500px]">
        @if (activeTab() === 'overview') {
          @if (scheduleService.isLoading()) {
            <div class="flex items-center justify-center h-full text-gray-500">Loading schedule...</div>
          } @else {
            <div class="flex flex-col border border-gray-200 rounded-lg overflow-hidden bg-white">
              @for (hour of workHours; track hour) {
                <div class="flex border-b border-gray-100 last:border-0 min-h-[80px]">
                  <div class="w-20 bg-gray-50 p-3 border-r border-gray-200 flex items-start justify-center">
                    <span class="text-sm font-bold text-gray-600">{{ hour }}</span>
                  </div>
                  <div class="flex-1 p-2 space-y-2">
                    @for (event of getEventsForHour(hour); track event.id) {
                      <div class="p-3 rounded border shadow-sm"
                           [ngClass]="{'bg-blue-50 border-blue-400': event.type === 'Appointment', 'bg-orange-50 border-orange-400': event.type === 'Rest'}">
                        
                        <div class="flex justify-between items-start">
                          <span class="text-xs px-2 py-1 rounded text-white font-semibold shadow-sm"
                                [ngClass]="event.type === 'Appointment' ? 'bg-blue-500' : 'bg-orange-500'">
                            {{ event.startTime | date:'shortTime' }} - {{ event.endTime | date:'shortTime' }}
                            ({{ event.type === 'Appointment' ? 'Cita' : 'Descanso' }})
                          </span>
                        </div>
                        
                        @if (event.type === 'Appointment') {
                          <div class="flex justify-between items-end mt-2">
                            <div>
                              <p class="text-sm text-gray-800"><strong>Patient:</strong> {{ event.patientName }}</p>
                              <p class="text-xs text-gray-600 mt-1"><strong>Reason:</strong> {{ event.scheduleDescription }}</p>
                            </div>
                            
                            <div>
                              @if (event.scheduleStatus === 'Scheduled') {
                                <button (click)="startEncounter(event.id)"
                                        class="bg-blue-600 text-white px-3 py-1.5 rounded text-xs font-bold hover:bg-blue-700 transition shadow-sm">
                                  Start Encounter
                                </button>
                              }
                              @if (event.scheduleStatus === 'InProgress') {
                                <span class="text-yellow-700 bg-yellow-100 px-2 py-1 rounded text-xs font-bold border border-yellow-300">
                                  In Progress
                                </span>
                              }
                              @if (event.scheduleStatus === 'Completed') {
                                <span class="text-green-700 bg-green-100 px-2 py-1 rounded text-xs font-bold border border-green-300">
                                  Completed
                                </span>
                              }
                            </div>
                          </div>
                        } @else {
                          <p class="mt-2 text-sm text-gray-800"><strong>Reason:</strong> {{ event.scheduleDescription }}</p>
                        }
                      </div>
                    }
                    @if (getEventsForHour(hour).length === 0) {
                      <div class="h-full w-full flex items-center p-2 text-gray-300 text-xs italic">
                        Available
                      </div>
                    }
                  </div>
                </div>
              }
            </div>
          }
        }

        @if (activeTab() === 'new') {
          <div class="flex flex-col gap-5 max-w-lg mx-auto mt-2">
            <div class="relative">
              <label class="block text-sm font-medium text-gray-700 mb-1">Search Patient (Name or ID)</label>
              <input type="text" [(ngModel)]="patientSearchTerm" (ngModelChange)="onSearchPatient()" placeholder="e.g., John Doe or ID..."
                     class="block w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 shadow-sm">
              
              @if (patientSearchResults().length > 0) {
                <div class="absolute z-10 w-full mt-1 bg-white border border-gray-200 rounded-md shadow-lg max-h-48 overflow-y-auto">
                  @for (p of patientSearchResults(); track p.id) {
                    <div (click)="selectPatient(p)" class="p-3 hover:bg-blue-50 cursor-pointer border-b last:border-0 text-sm">
                      <strong>{{ p.firstName }} {{ p.lastName }}</strong> <span class="text-gray-500 ml-2">(ID: {{ p.id }})</span>
                    </div>
                  }
                </div>
              }
            </div>

            @if (selectedPatient()) {
              <div class="bg-blue-50 border border-blue-200 p-3 rounded-md flex justify-between items-center shadow-sm">
                <div>
                  <p class="text-xs text-blue-600 font-semibold uppercase tracking-wider">Selected Patient</p>
                  <p class="text-sm text-blue-900 font-medium mt-1">{{ selectedPatient()?.firstName }} {{ selectedPatient()?.lastName }}</p>
                </div>
                <button (click)="clearPatient()" class="text-blue-500 hover:text-blue-700 font-bold text-xl px-2">&times;</button>
              </div>
            }

            <div class="flex gap-4">
              <div class="flex-1">
                <label class="block text-sm font-medium text-gray-700 mb-1">Start Time</label>
                <input type="time" [(ngModel)]="startTime" 
                       class="block w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 shadow-sm">
              </div>
              <div class="flex-1">
                <label class="block text-sm font-medium text-gray-700 mb-1">End Time</label>
                <input type="time" [(ngModel)]="endTime" 
                       class="block w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 shadow-sm">
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Reason for Visit</label>
              <textarea [(ngModel)]="reason" rows="3" placeholder="e.g., Routine checkup..."
                        class="block w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 shadow-sm"></textarea>
            </div>
            
            @if (errorMessage()) {
              <div class="p-3 bg-red-50 text-red-700 text-sm rounded-md border border-red-200">
                {{ errorMessage() }}
              </div>
            }

            <div class="mt-2 flex justify-end">
              <button (click)="saveAppointment()" [disabled]="isSaving() || !selectedPatient()"
                      class="w-full md:w-auto px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition font-medium disabled:opacity-50 disabled:cursor-not-allowed shadow-sm">
                {{ isSaving() ? 'Saving...' : 'Schedule Appointment' }}
              </button>
            </div>
          </div>
        }
      </div>

      <div class="p-4 border-t border-gray-200 flex justify-end bg-gray-50 rounded-b-lg">
        <button mat-dialog-close class="px-4 py-2 bg-white border border-gray-300 text-gray-700 rounded hover:bg-gray-100 font-medium transition shadow-sm">
          Close
        </button>
      </div>
    </div>
  `
})
export class DailyScheduleModalComponent implements OnInit {
  data = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<DailyScheduleModalComponent>);
  scheduleService = inject(ScheduleService);
  http = inject(HttpClient);

  encounterService = inject(EncounterService);
  dialog = inject(MatDialog);
  
  activeTab = signal<'overview' | 'new'>('overview');

  workHours = Array.from({length: 11}, (_, i) => {
    const hour = i + 8;
    return `${hour.toString().padStart(2, '0')}:00`;
  });

  startTime = signal<string>('09:00');
  endTime = signal<string>('09:30');
  reason = signal<string>('');
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');

  patientSearchTerm = signal<string>('');
  patientSearchResults = signal<any[]>([]);
  selectedPatient = signal<any | null>(null);

  constructor() {
    effect(() => {
      this.scheduleService.loadSchedule();
    });
  }

  ngOnInit() {
    this.scheduleService.selectedDoctorId.set(this.data.doctorId);
  }

  onDateChange(dateString: string) {
    if (dateString) {
      const parts = dateString.split('-');
      const targetDate = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]), 12, 0, 0);
      this.scheduleService.selectedDate.set(targetDate);
    }
  }

  getEventsForHour(hourString: string) {
    const events = this.scheduleService.events();
    const blockHour = parseInt(hourString.split(':')[0]);

    return events.filter(e => {
      const timeParts = e.startTime.split('T')[1].split(':');
      const eventStartHour = parseInt(timeParts[0]);
      
      return eventStartHour === blockHour;
    });
  }


  startEncounter(appointmentId: number) {
    if (!confirm('Do you want to start the medical encounter for this appointment?')) return;

    this.encounterService.startEncounter(appointmentId).subscribe({
      next: () => {
        alert('Encounter started successfully. The doctor can now view it in their panel.');
        this.scheduleService.loadSchedule(); // Only refresh the table
      },
      error: (err) => {
        console.error('Error starting encounter:', err);
        alert('Failed to start the encounter.');
      }
    });
  }

  onSearchPatient() {
    const term = this.patientSearchTerm().trim();
    if (term.length < 1) {
      this.patientSearchResults.set([]);
      return;
    }

    const headers = { Authorization: `Bearer ${getPmsToken()}` };
    
    this.http.get<any[]>(`${environment.apiUrl}/api/patients/search?searchTerm=${term}`, { headers }).subscribe({
      next: (results) => this.patientSearchResults.set(results),
      error: (err) => console.error('Error searching patient:', err)
    });
  }

  selectPatient(patient: any) {
    this.selectedPatient.set(patient);
    this.patientSearchTerm.set('');
    this.patientSearchResults.set([]);
  }

  clearPatient() {
    this.selectedPatient.set(null);
  }

  saveAppointment() {
    if (!this.selectedPatient() || !this.reason() || !this.startTime() || !this.endTime()) {
      this.errorMessage.set('Please fill all fields and select a patient.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');

    const year = this.scheduleService.selectedDate().getFullYear();
    const month = (this.scheduleService.selectedDate().getMonth() + 1).toString().padStart(2, '0');
    const day = this.scheduleService.selectedDate().getDate().toString().padStart(2, '0');
    const dateStr = `${year}-${month}-${day}`;

    const startDateTime = `${dateStr}T${this.startTime()}:00`;
    const endDateTime = `${dateStr}T${this.endTime()}:00`;

    const newAppointment = {
      startTime: startDateTime,
      endTime: endDateTime,
      status: 'Scheduled',
      reason: this.reason(),
      doctorId: this.data.doctorId,
      patientId: this.selectedPatient().id
    };

    this.scheduleService.createAppointment(newAppointment).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.reason.set('');
        this.clearPatient();
        this.activeTab.set('overview');
        this.scheduleService.loadSchedule(); 
      },
      error: (err) => {
        console.error('Error saving appointment:', err);
        this.errorMessage.set('There was an error saving the appointment.');
        this.isSaving.set(false);
      }
    });
  }
}