using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebServices.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Patient> DBPatients { get; set; }

        public DbSet<Doctor> DBDoctors { get; set; }

        public DbSet<MedicalRecord> DBMedicalRecords { get; set; }

        public DbSet<Appointment> DBAppointments { get; set; }

        public DbSet<Prescriptions> DBPrescriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Patient>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Doctor>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Appointment>()
                .HasKey(ap => ap.Id);

            modelBuilder.Entity<Appointment>()
                .Property(ap => ap.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Appointment>()
                .HasOne(ap => ap.Doctor)
                .WithMany(d => d.Appointments);

            modelBuilder.Entity<Appointment>()
                .HasOne(p => p.Patient)
                .WithMany(ap => ap.Appointments);

            modelBuilder.Entity<MedicalRecord>()
                .HasKey(mr => mr.Id);

            modelBuilder.Entity<MedicalRecord>()
                .Property(mr => mr.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithMany(p => p.MedicalRecords);

            modelBuilder.Entity<Prescriptions>()
                .HasKey(pr => pr.Id);

            modelBuilder.Entity<Prescriptions> ()
                .Property(pr => pr.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Prescriptions>()
                .HasOne(pr => pr.Doctor)
                .WithMany(d => d.Prescriptions);

            modelBuilder.Entity<Prescriptions>()
                .HasOne(pr => pr.Patient)
                .WithMany(p => p.Prescriptions);
        }
    }
}
