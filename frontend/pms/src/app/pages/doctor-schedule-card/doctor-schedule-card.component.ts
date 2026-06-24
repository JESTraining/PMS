import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-doctor-schedule-card.component',
  imports: [],
  templateUrl: './doctor-schedule-card.component.html',
  styleUrl: './doctor-schedule-card.component.css',
})
export class DoctorScheduleCardComponent {
  @Input() doctor: DoctorScheduleInterface = {
    id: 0,
    name: '',
    appointments: 0,
    availableAppointments: 0
  };
}
