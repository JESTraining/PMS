
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EncounterService } from '../../services/encounter/encounter.service';
import { EncounterInterface } from '../../Entities/Encounters/Encounter';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-encounter-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './encounter-detail.component.html',
})
export class EncounterDetailComponent implements OnInit {

  encounter = signal<EncounterInterface | null>(null);
  loading = signal<boolean>(false);

  // =========================
  // 📝 NOTES
  // =========================

  editingSection = signal<string | null>(null);
  editedNote = signal<any>(null);

  // =========================
  // 🧪 OBSERVATIONS
  // =========================

  editingObservationId = signal<number | null>(null);
  editedObservation = signal<any>(null);
  creatingObservation = signal<boolean>(false);
  newObservation = signal<any>({
    category: '',
    displayName: '',
    valueQuantity: '',
    unit: ''
  });

  editingConditionId = signal<number | null>(null);
  editedCondition = signal<any>(null);
  creatingCondition = signal<boolean>(false);
  newCondition = signal<any>({
      code: '',
      displayName: '',
      clinicalStatus: 'Active'
    });

  editingPrescriptionId = signal<number | null>(null);
  editedPrescription = signal<any>(null);
  creatingPrescription = signal<boolean>(false);
  newPrescription = signal<any>({
    issueDate: '',
    medications: []
  });
  medicationsCatalog = signal<any[]>([]);

  editingAppointment = signal<boolean>(false);
  editedAppointment = signal<any>(null);

  constructor(
    private route: ActivatedRoute,
    private encounterService: EncounterService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadEncounter(id);
    this.loadMedications();
  }
  loadMedications() {
  this.encounterService.getMedications().subscribe({
    next: (res) => this.medicationsCatalog.set(res),
    error: (err) => console.error(err)
  });
}

  loadEncounter(id: number): void {
    this.loading.set(true);

    this.encounterService.getEncounterDetail(id).subscribe({
      next: (data) => {
        console.log('OBSERVACIONES:', data.clinicalObservations);
        console.log("condition", data.conditions);
        console.log("prescription", data.prescriptions);
        this.encounter.set(data);
      },
      error: (err) => console.error(err),
      complete: () => this.loading.set(false)
    });
  }

  // =========================
  // 📝 NOTES
  // =========================

  startEdit(section: string, data?: any) {
    this.editingSection.set(section);

    if (section === 'notes') {
      this.editedNote.set({ ...data });
    }
  }

  cancelEdit() {
    this.editingSection.set(null);
    this.editedNote.set(null);
  }

  saveNote() {
    const e = this.encounter();
    if (!e) return;

    this.encounterService
      .updateClinicalNote(e.EncounterId, this.editedNote())
      .subscribe({
        next: () => this.reload(),
        error: (err) => console.error(err)
      });
  }
  // =========================
  // 📅 APPOINTMENT
  // =========================
    startAppointmentEdit(e: any) {
      this.editingAppointment.set(true);

      this.editedAppointment.set({
        reason: e.appointment.reason,
        startTime: e.appointment.startTime,
        endTime: e.appointment.endTime
      });
    }
  cancelAppointmentEdit() {
    this.editingAppointment.set(false);
    this.editedAppointment.set(null);
  }
  saveAppointment() {
    const e = this.encounter();
    if (!e) return;

    const a = this.editedAppointment();

    const payload = {
      reason: a.reason,
      startTime: a.startTime,
      endTime: a.endTime
    };

    this.encounterService
      .updateAppointment(e.EncounterId, payload)
      .subscribe({
        next: () => {
          this.reload();
          this.cancelAppointmentEdit();
        },
        error: (err) => console.error(err)
      });
  }


  // =========================
  // 🧪 OBSERVATIONS
  // =========================
  startCreateObservation() {
    this.creatingObservation.set(true);

    this.newObservation.set({
      category: '',
      displayName: '',
      valueQuantity: '',
      unit: ''
    });
  }

  cancelCreateObservation() {
    this.creatingObservation.set(false);
  }

  saveNewObservation() {
    const e = this.encounter();
    if (!e) return;

    const obs = this.newObservation();

    const payload = {
      category: obs.category,
      displayName: obs.displayName,
      valueString: String(obs.valueQuantity),
      unit: obs.unit
    };

    this.encounterService
      .addObservation(e.EncounterId, payload)
      .subscribe({
        next: () => {
          this.reload();
          this.creatingObservation.set(false);
        },
        error: (err) => console.error(err)
      });
  }

  startObservationEdit(obs: any) {
    this.editingObservationId.set(obs.id);
    this.editedObservation.set({ ...obs });
  }

  cancelObservationEdit() {
    this.editingObservationId.set(null);
    this.editedObservation.set(null);
  }

  saveObservation(obsId: number) {
    const e = this.encounter();
    if (!e) return;

    const obs = this.editedObservation();

    const payload = {
      category: obs.category,
      displayName: obs.displayName,
      valueString: String(obs.valueQuantity),
      unit: obs.unit
    };

    this.encounterService
      .updateObservation(e.EncounterId, obsId, payload)
      .subscribe({
        next: () => {
          this.reload();
          this.cancelObservationEdit();
        },
        error: (err) => console.error(err)
      });
  }

  deleteObservation(obsId: number) {
    const e = this.encounter();
    if (!e) return;

    this.encounterService
      .deleteObservation(e.EncounterId, obsId)
      .subscribe({
        next: () => this.reload(),
        error: (err) => console.error(err)
      });
  }

  // =========================
  // 🩺 CONDITIONS
  // =========================
  startCreateCondition() {
    this.creatingCondition.set(true);

    this.newCondition.set({
      code: '',
      displayName: '',
      clinicalStatus: 'Active'
    });
  }

  cancelCreateCondition() {
    this.creatingCondition.set(false);
  }

  saveNewCondition() {
    const e = this.encounter();
    if (!e) return;

    const cond = this.newCondition();

    const payload = {
      code: cond.code,
      displayName: cond.displayName,
      clinicalStatus: cond.clinicalStatus
    };

    this.encounterService
      .addCondition(e.EncounterId, payload)
      .subscribe({
        next: () => {
          this.reload();
          this.creatingCondition.set(false);
        },
        error: (err) => console.error(err)
      });
  }

    startConditionEdit(cond: any) {
    this.editingConditionId.set((cond.id));
    this.editedCondition.set({ ...cond });
  }

  cancelConditionEdit() {
    this.editingConditionId.set(null);
    this.editedCondition.set(null);
  }

  saveCondition(conditionId: number) {
    const e = this.encounter();
    if (!e) return;

    const cond = this.editedCondition();

    const payload = {
      code: cond.code,
      displayName: cond.displayName,
      clinicalStatus: cond.clinicalStatus
    };
    console.log("PAYLOAD CONDITION:", payload);

    this.encounterService
      .updateCondition(e.EncounterId, conditionId, payload)
      .subscribe({
        next: () => {
          this.reload();
          this.cancelConditionEdit();
        },
        error: (err) => console.error(err)
      });
  }
  deleteCondition(conditionId: number) {
    const e = this.encounter();
    if (!e) return;

    this.encounterService
      .deleteCondition(e.EncounterId, conditionId)
      .subscribe({
        next: () => this.reload(),
        error: (err) => console.error(err)
      });
  }
  startPrescriptionEdit(p: any) {
  this.editingPrescriptionId.set(p.id);
  this.editedPrescription.set({ ...p });
}

cancelPrescriptionEdit() {
  this.editingPrescriptionId.set(null);
  this.editedPrescription.set(null);
}

savePrescription(prescriptionId: number) {
  const e = this.encounter();
  if (!e) return;

  const p = this.editedPrescription();

  const payload = {
    issueDate: p.issueDate,
    medications: p.medications.map((m: any) => ({
      id: m.id,
      dosage: m.dosage,
      refills: m.refills,
      medicationId: m.medication.id
    }))
  };

  this.encounterService
    .updatePrescription(e.EncounterId, prescriptionId, payload)
    .subscribe({
      next: () => {
        this.reload();
        this.cancelPrescriptionEdit();
      },
      error: (err) => console.error(err)
    });
  }
  startCreatePrescription() {
    this.creatingPrescription.set(true);

    this.newPrescription.set({
      issueDate: '',
      medications: []
    });
  }

  cancelCreatePrescription() {
    this.creatingPrescription.set(false);
  }

  addMedicationRow() {
    const current = this.newPrescription();
    current.medications.push({
      medicationId: 0,
      dosage: '',
      refills: 0
    });

    this.newPrescription.set({ ...current });
  }

  removeMedicationRow(index: number) {
    const current = this.newPrescription();
    current.medications.splice(index, 1);

    this.newPrescription.set({ ...current });
  }
  saveNewPrescription() {
    const e = this.encounter();
    if (!e) return;

    const p = this.newPrescription();

    const payload = {
      issueDate: p.issueDate,
      medications: p.medications.map((m: any) => ({
        medicationId: m.medicationId,
        dosage: m.dosage,
        refills: m.refills
      }))
    };

    this.encounterService
      .createPrescription(e.EncounterId, payload)
      .subscribe({
        next: () => {
          this.reload();
          this.creatingPrescription.set(false);
        },
        error: (err) => console.error(err)
        
      });
  }
  deletePrescription(prescriptionId: number) {
    const e = this.encounter();
    if (!e) return;

    this.encounterService
      .deletePrescription(e.EncounterId, prescriptionId)
      .subscribe({
        next: () => this.reload(),
        error: (err) => console.error(err)
      });
  }

  // =========================
  // 🔁 HELPERS
  // =========================

  reload() {
    const id = this.encounter()?.EncounterId;
    if (!id) return;

    this.loadEncounter(id);
  }
}

