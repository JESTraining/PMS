import { Prescription } from '../../models/prescription.model';
import { AppointmentInterface } from './appointment';
export interface EncounterListItem {
  encounterId: number;
  patientName: string;
  startTime: string;
  endTime: string;
  encounterReason: string;
}
export interface EncounterInterface {
  EncounterId: number;
  patientName: string;

  // 🔥 ENCOUNTER (real)
  startTime: string;
  endTime: string | null;
  encounterReason: string;

  // 🔥 APPOINTMENT (separado)
  appointment: AppointmentInterface;

  // 🔥 resto
  conditions: conditionInterface[];
  clinicalObservations: clinicalObservationsInterface[];
  clinicalNotes: clinicalNotesInterface[];
  prescriptions: Prescription[];
}
export interface EncounterSummaryDto {
  EncounterId: number;
  patientName: string;
  subjective: string;
  objective: string;
  assessment: string;
  plan: string;
  observationsCount: number;
  conditionsCount: number;
  allergiesCount: number;
  prescriptionsCount: number;
  observations: any[]; 
  allergies: any[];
}