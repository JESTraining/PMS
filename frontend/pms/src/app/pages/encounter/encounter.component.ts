import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EncounterService } from '../../services/encounter/encounter.service';
import { EncounterListItem } from '../../Entities/Encounters/Encounter';

@Component({
  selector: 'app-encounter',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './encounter.component.html',
})
export class EncounterComponent implements OnInit {

  encounters = signal<EncounterListItem[]>([]);
  loading = signal<boolean>(false);

  constructor(
    private encounterService: EncounterService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadEncounters();
  }

  loadEncounters(): void {
    this.loading.set(true);

    this.encounterService.getEncounters().subscribe({
      next: (data) => {
        this.encounters.set(data);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => this.loading.set(false)
    });
  }

  goToDetail(id: number): void {
    this.router.navigate(['/encounters', id]);
  }
}