using Domain.Entities;
using Domain.SharedConstants;
using Microsoft.EntityFrameworkCore;

namespace WebServices.DataAccess
{
    /// <summary>
    /// Represents the main database context for the Patient Management System (PMS).
    /// Manages entity configurations, relationships, and database sets.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        // =========================================================
        // SYSTEM ENTITIES
        // =========================================================

        /// <summary>System users including Administrators, Patients, and Doctors.</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Registered patients in the system.</summary>
        public DbSet<Patient> DBPatients { get; set; }

        /// <summary>Registered healthcare providers (Doctors).</summary>
        public DbSet<Doctor> DBDoctors { get; set; }

        // =========================================================
        // BILLING & INSURANCE ENTITIES
        // =========================================================

        /// <summary>Financial invoices billed to patients.</summary>
        public DbSet<Invoice> DBInvoices { get; set; }

        /// <summary>Payments registered against financial invoices.</summary>
        public DbSet<Payment> DBPayments { get; set; }

        /// <summary>Medical insurance policies linked to patients.</summary>
        public DbSet<Insurance> DBInsurances { get; set; }

        // =========================================================
        // SCHEDULING ENTITIES
        // =========================================================

        /// <summary>Scheduled medical appointments.</summary>
        public DbSet<Appointment> DBAppointments { get; set; }

        /// <summary>Recurring daily break times for doctors.</summary>
        public DbSet<DoctorRestSchedule> DBDoctorRestSchedules { get; set; }

        /// <summary>Read-only SQL View combining appointments for the calendar UI.</summary>
        public DbSet<ScheduleView> DBScheduleViews { get; set; }

        // =========================================================
        // CLINICAL WORKFLOW ENTITIES (FHIR Inspired)
        // =========================================================

        /// <summary>The main clinical encounter/visit record connecting a patient and doctor.</summary>
        public DbSet<Encounter> Encounters { get; set; }

        /// <summary>Vital signs and objective measurements taken during an encounter.</summary>
        public DbSet<ClinicalObservation> ClinicalObservations { get; set; }

        /// <summary>SOAP narrative notes written by the doctor during an encounter.</summary>
        public DbSet<ClinicalNote> ClinicalNotes { get; set; }

        /// <summary>Medical diagnoses or problems identified.</summary>
        public DbSet<Condition> Conditions { get; set; }

        /// <summary>Medical interventions or services performed.</summary>
        public DbSet<Procedure> Procedures { get; set; }

        /// <summary>Allergies or adverse reactions registered for a patient.</summary>
        public DbSet<AllergyIntolerance> AllergyIntolerances { get; set; }

        /// <summary>Medication prescriptions issued by a doctor to a patient.</summary>
        public DbSet<Prescriptions> DBPrescriptions { get; set; }

        /// <summary>Join entity representing the many-to-many relationship between Prescriptions and Medications, including dosage and refills.</summary>
        public DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }

        /// <summary>Master list of medications available for prescription.</summary>
        public DbSet<Medication> Medications { get; set; }

        /// <summary>Laboratory tests ordered during an encounter.</summary>
        public DbSet<Laboratory> Laboratories { get; set; }

        /// <summary>Join entity representing the relationship between an Encounter and its ordered laboratory tests, including results and interpretations.</summary>
        public DbSet<EncounterLaboratories> EncounterLaboratories { get; set; }

        /// <summary>Detailed results and interpretations for each laboratory test ordered during an encounter.</summary>
        public DbSet<EncounterLaboratoriesDetail> EncounterLaboratoriesDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // SYSTEM & USERS CONFIGURATION
            // =========================================================
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = -1,
                    Name = "PMS Administrator",
                    Email = "adminPMS@unosquare.com",
                    IsActive = true,
                    UserName = "admin",
                    Password = "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", // Control123 (HEX codification)
                    CreationDate = new DateTime(2026, 4, 9),
                    Role = UserConstants.RoleConstants.AdminRole,
                },
                new User
                {
                    Id = -2,
                    Name = "PMS Patient",
                    Email = "patientPMS@unosquare.com",
                    IsActive = true,
                    UserName = "patient",
                    Password = "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", // Control123 (HEX codification)
                    CreationDate = new DateTime(2026, 4, 9),
                    Role = UserConstants.RoleConstants.PatientRole,
                },
                new User
                {
                    Id = -3,
                    Name = "PMS Doctor",
                    Email = "doctorPMS@unosquare.com",
                    IsActive = true,
                    UserName = "doctor",
                    Password = "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", // Control123 (HEX codification)
                    CreationDate = new DateTime(2026, 4, 9),
                    Role = UserConstants.RoleConstants.DoctorRole,
                }
            );

            modelBuilder.Entity<Patient>().HasKey(p => p.Id);
            modelBuilder.Entity<Patient>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = -1,
                    FirstName = "Juan",
                    LastName = "Perez",
                    DateOfBirth = new DateTime(1990, 5, 10),
                    Phone = "12345678",
                    Email = "juan@test.com",
                    CreatedAt = new DateTime(2026, 4, 30)
                }
            );

            modelBuilder.Entity<Doctor>().HasKey(p => p.Id);
            modelBuilder.Entity<Doctor>().Property(d => d.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = 1,
                    Name = "Aurelio",
                    Specialty = "Pediatrics",
                    UserId = -3
                }
            );
            modelBuilder.Entity<Doctor>().HasData(DoctorSeedData.GetDoctors());
            modelBuilder.Entity<User>().HasData(DoctorSeedData.GetUsers());
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // INVOICES & INSURANCE CONFIGURATION
            // =========================================================
            modelBuilder.Entity<Invoice>().HasKey(i => i.Id);
            modelBuilder.Entity<Invoice>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Invoice>().Property(i => i.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.PaidAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices);
            modelBuilder.Entity<Invoice>()
                .HasIndex("PatientId")
                .HasFilter("[Status] IN (1, 3)")
                .IsUnique();
            modelBuilder.Entity<InvoiceDetail>().Property(i => i.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<InvoiceDetail>().Property(i => i.Price).HasPrecision(18, 2);

            modelBuilder.Entity<Payment>().HasKey(p => p.Id);
            modelBuilder.Entity<Payment>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Insurance>().HasKey(i => i.Id);
            modelBuilder.Entity<Insurance>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Insurance>().Property(i => i.OfficeVisitCopay).HasPrecision(18, 2);
            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Insurances)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // SCHEDULING CONFIGURATION
            // =========================================================
            modelBuilder.Entity<Appointment>().HasKey(ap => ap.Id);
            modelBuilder.Entity<Appointment>().Property(ap => ap.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Appointment>()
                .HasOne(ap => ap.Doctor)
                .WithMany(d => d.Appointments);
            modelBuilder.Entity<Appointment>()
                .HasOne(p => p.Patient)
                .WithMany(ap => ap.Appointments);

            modelBuilder.Entity<DoctorRestSchedule>().HasKey(dr => dr.Id);
            modelBuilder.Entity<DoctorRestSchedule>().Property(dr => dr.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<DoctorRestSchedule>()
                .HasOne(dr => dr.Doctor)
                .WithMany(d => d.DoctorRestSchedules);

            modelBuilder.Entity<DoctorRestSchedule>().HasData(
                new
                {
                    Id = 1,
                    DoctorId = 1,
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    Reason = "Lunch Break"
                }
            );

            modelBuilder.Entity<ScheduleView>(sv =>
            {
                sv.HasNoKey();
                sv.ToSqlQuery<ScheduleView>("SELECT 'Appointment' AS Type," +
                    "a.Id," +
                    "CAST(a.StartTime AS Date) AS Date," +
                    "a.StartTime," +
                    "a.EndTime," +
                    "a.Status AS ScheduleStatus," +
                    "a.Reason AS ScheduleDescription," +
                    "a.DoctorId," +
                    "d.Name AS DoctorName," +
                    "a.PatientId," +
                    "p.FirstName + ' ' + p.LastName AS PatientName " +
                    "FROM DBAppointments a " +
                    "JOIN DBDoctors d ON a.DoctorId = d.Id " +
                    "JOIN DBPatients p ON a.PatientId = p.Id");
            });

            // =========================================================
            // CLINICAL WORKFLOW CONFIGURATION (Encounter, Observations, etc.)
            // =========================================================

            // 1. Enum String Conversions (For database readability)
            modelBuilder.Entity<Encounter>().Property(e => e.Status).HasConversion<string>();
            modelBuilder.Entity<Procedure>().Property(p => p.Status).HasConversion<string>();
            modelBuilder.Entity<Condition>().Property(c => c.ClinicalStatus).HasConversion<string>();
            modelBuilder.Entity<AllergyIntolerance>().Property(a => a.Criticality).HasConversion<string>();
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            // 2. Decimal Precision (Prevents data loss warnings)
            modelBuilder.Entity<ClinicalObservation>()
                .Property(c => c.ValueQuantity)
                .HasPrecision(18, 2);

            // 3. Encounter Relationships and Audit Protection (Delete Behaviors)
            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Patient)
                .WithMany(p => p.Encounters)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Doctor)
                .WithMany()
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Encounter>()
                .HasMany(e => e.Laboratories)
                .WithOne(l => l.Encounter);

            // 4. Prescriptions unified configurations
            modelBuilder.Entity<Prescriptions>().HasKey(pr => pr.Id);
            modelBuilder.Entity<Prescriptions>().Property(pr => pr.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Prescriptions>()
                .HasOne(pr => pr.Doctor)
                .WithMany(d => d.Prescriptions);

            modelBuilder.Entity<Prescriptions>()
                .HasOne(pr => pr.Patient)
                .WithMany(p => p.Prescriptions);

            modelBuilder.Entity<Prescriptions>()
                .HasOne(p => p.Encounter)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(p => p.EncounterId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Medication>().HasKey(m => m.Id);

            modelBuilder.Entity<Medication>().Property(m => m.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Medication>().Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<Medication>().HasData(
                new Medication { Id = 1, Name = "Paracetamol" },
                new Medication { Id = 2, Name = "Ibuprofeno" }
            );
            modelBuilder.Entity<Prescriptions>().HasData(
                new Prescriptions
                {
                    Id = 1,
                    IssueDate = new DateOnly(2026, 4, 30),
                    EncounterId = null,
                    
                    DoctorId = 1,  
                    PatientId = 1
                }
            );
            //medications prescricions
            modelBuilder.Entity<PrescriptionMedication>()
            .HasKey(pm => pm.Id);

            // 🔥 relación con Prescription
            modelBuilder.Entity<PrescriptionMedication>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.Medications)
                .HasForeignKey(pm => pm.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔥 NUEVO: relación con Medication
            modelBuilder.Entity<PrescriptionMedication>()
                .HasOne(pm => pm.Medication)
                .WithMany() 
                .HasForeignKey(pm => pm.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PrescriptionMedication>().HasData(
                new PrescriptionMedication
                {
                    Id = 1,
                    MedicationId = 1,
                    Dosage = "500mg cada 8 horas",
                    Refills = 2,
                    PrescriptionId = 1
                },
                new PrescriptionMedication
                {
                    Id = 2,
                    MedicationId = 2,
                    Dosage = "400mg cada 12 horas",
                    Refills = 1,
                    PrescriptionId = 1
                }
            );

            // 5. Laboratory Entity Configuration
            modelBuilder.Entity<Laboratory>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Laboratory>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Laboratory>()
                .HasMany(l => l.EncounterLaboratoriesDetail)
                .WithOne(l => l.Laboratory);

            modelBuilder.Entity<EncounterLaboratories>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<EncounterLaboratories>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<EncounterLaboratories>()
                .HasMany(l => l.LaboratoriesDetails)
                .WithOne(d => d.EncounterLaboratories);

            modelBuilder.Entity<EncounterLaboratoriesDetail>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<EncounterLaboratoriesDetail>()
                .Property(d => d.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
