/**
 * Specialty is not yet stored on the Doctor entity. Until the backend provides it,
 * derive a stable mock specialty from the doctor id so the UI can group/filter by it.
 */
export const SPECIALTIES = ['General Medicine', 'Pediatrics', 'Cardiology', 'Dermatology', 'Orthopedics'] as const;

export type Specialty = (typeof SPECIALTIES)[number];

export function mockSpecialty(doctorId: number): Specialty {
  return SPECIALTIES[Math.abs(doctorId) % SPECIALTIES.length];
}
