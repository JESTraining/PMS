export interface DoctorOption {
  id: number;
  name: string;
  specialty?: string;
}

/** Doctors grouped under a specialty heading for the search dropdown. */
export interface SpecialtyGroup {
  specialty: string;
  doctors: DoctorOption[];
}

export interface GpOption extends DoctorOption {
  nextSlot: string;
}
