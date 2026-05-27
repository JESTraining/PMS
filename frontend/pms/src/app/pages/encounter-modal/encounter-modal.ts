import { Component, Input, Output, EventEmitter, OnChanges, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { EncounterInterface } from '../../Entities/Encounters/Encounter';

@Component({
  selector: 'app-encounter-modal',
  imports: [CommonModule, MatFormFieldModule, FormsModule],
  templateUrl: './encounter-modal.html',
  styleUrl: './encounter-modal.css',
})

export class EncounterModal implements OnChanges {
  @Input() encounter: EncounterInterface = {
    EncounterId: 0,
    patientName: '',
    startTime: '',
    endTime: '',
    encounterReason: '',
    appointment: {
      id: 0,
      reason: '',
      startTime: '',
      endTime: ''
    },

    conditions: [],
    clinicalObservations: [],
    clinicalNotes: [],
    prescriptions: []
  };

  @Output() save = new EventEmitter<EncounterFormInterface>();
  @Output() close = new EventEmitter<void>();

  encounterForm: EncounterFormInterface = {
    EncounterId: this.encounter.EncounterId,
    ClinicalNotes: '',
    Alergies: '',
    Prescription: ''
  };

  formData = signal<EncounterFormInterface>({ ...this.encounterForm });
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');

  ngOnChanges(): void {
    this.formData.set({ ...this.encounterForm });
    this.errorMessage.set('');
  }

  onSave(): void {
    if (!this.validateForm()) {
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');

    this.save.emit(this.formData());
    this.isSaving.set(false);
  }

  onClose(): void {
    this.close.emit();
  }

  private validateForm(): boolean {
    const formValue = this.formData();

    if (!formValue.ClinicalNotes || formValue.ClinicalNotes.trim() === '') {
      this.errorMessage.set('Clinical notes are required.');
      return false;
    }

    if (!formValue.Alergies || formValue.Alergies.trim() === '') {
      this.errorMessage.set('Allergies are required.');
      return false;
    }

    if (!formValue.Prescription || formValue.Prescription.trim() === '') {
      this.errorMessage.set('Prescription is required.');
      return false;
    }

    return true;
  }

  updateFormField(field: keyof EncounterFormInterface, value: any): void {
    const current = this.formData();
    this.formData.set({ ...current, [field]: value });
  }
}
