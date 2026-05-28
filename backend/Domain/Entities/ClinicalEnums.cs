using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the current state of the clinical encounter workflow.
    /// </summary>
    public enum EncounterStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }

    /// <summary>
    /// Represents the execution state of a medical procedure.
    /// </summary>
    public enum ProcedureStatus
    {
        Planned,
        InProgress,
        Aborted,
        Completed
    }

    /// <summary>
    /// Represents the clinical status of a patient's diagnosed condition or problem.
    /// </summary>
    public enum ConditionClinicalStatus
    {
        Active,
        Recurrence,
        Relapse,
        Inactive,
        Remission,
        Resolved
    }

    /// <summary>
    /// Represents the potential severity of an allergic reaction.
    /// </summary>
    public enum AllergyCriticality
    {
        Low,
        High,
        UnableToAssess
    }

    /// <summary>
    /// Represents the current status of a scheduled appointment between a patient and a healthcare provider.
    /// </summary>
    public enum AppointmentStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

    public enum LaboratoryStatus
    {
        Requested,
        InProgress,
        Completed,
        Delivered,
    }
}