export interface DashboardSummary {
    totalAppointments: number;
    totalEncounters: number;
    newPatients: number;
    totalEarnings: number;
    completedAppointments: number;
    upcomingAppointments: number;
    completedEncounters: number;
    topConditions: { conditionName: string, patientCount: number }[];
    monthlyAppointments: { patientName: string, type: string, doctorName: string, time: string }[];
    calendarActiveDays: { day: number, hasCompleted: boolean, hasScheduled: boolean }[];
}

export interface DashboardSparklines {
    dates: string[];
    appointmentsHistory: number[];
    encountersHistory: number[];
    patientsHistory: number[];
    earningsHistory: number[];
}