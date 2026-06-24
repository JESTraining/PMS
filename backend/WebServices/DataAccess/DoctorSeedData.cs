using Domain.Entities;
using Domain.SharedConstants;

namespace WebServices.DataAccess
{
    /// <summary>
    /// Seed data for the 9 additional doctor users and their Doctor profiles.
    /// Doctor 1 (Aurelio, UserId = -3) is seeded inline in DatabaseContext.
    /// </summary>
    public static class DoctorSeedData
    {
        private const string HashedPassword = "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C";
        private static readonly DateTime SeedDate = new DateTime(2026, 6, 19);

        public static User[] GetUsers() =>
        [
            new User { Id = -4,  Name = "Sofia Ramirez",    Email = "sofia.ramirez@pms.com",    IsActive = true, UserName = "doctor2",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -5,  Name = "Carlos Mendez",    Email = "carlos.mendez@pms.com",    IsActive = true, UserName = "doctor3",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -6,  Name = "Ana Torres",       Email = "ana.torres@pms.com",       IsActive = true, UserName = "doctor4",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -7,  Name = "Luis Garcia",      Email = "luis.garcia@pms.com",      IsActive = true, UserName = "doctor5",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -8,  Name = "Maria Santos",     Email = "maria.santos@pms.com",     IsActive = true, UserName = "doctor6",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -9,  Name = "Elena Vasquez",    Email = "elena.vasquez@pms.com",    IsActive = true, UserName = "doctor7",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -10, Name = "Roberto Morales",  Email = "roberto.morales@pms.com",  IsActive = true, UserName = "doctor8",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -11, Name = "Carmen Jimenez",   Email = "carmen.jimenez@pms.com",   IsActive = true, UserName = "doctor9",  Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
            new User { Id = -12, Name = "Pablo Rodriguez",  Email = "pablo.rodriguez@pms.com",  IsActive = true, UserName = "doctor10", Password = HashedPassword, CreationDate = SeedDate, Role = UserConstants.RoleConstants.DoctorRole },
        ];

        public static Doctor[] GetDoctors() =>
        [
            new Doctor { Id = 2,  Name = "Sofia Ramirez",   Specialty = "General Medicine", UserId = -4  },
            new Doctor { Id = 3,  Name = "Carlos Mendez",   Specialty = "General Medicine", UserId = -5  },
            new Doctor { Id = 4,  Name = "Ana Torres",      Specialty = "Cardiology",       UserId = -6  },
            new Doctor { Id = 5,  Name = "Luis Garcia",     Specialty = "General Medicine", UserId = -7  },
            new Doctor { Id = 6,  Name = "Maria Santos",    Specialty = "Pediatrics",       UserId = -8  },
            new Doctor { Id = 7,  Name = "Elena Vasquez",   Specialty = "Neurology",        UserId = -9  },
            new Doctor { Id = 8,  Name = "Roberto Morales", Specialty = "Cardiology",       UserId = -10 },
            new Doctor { Id = 9,  Name = "Carmen Jimenez",  Specialty = "Dermatology",      UserId = -11 },
            new Doctor { Id = 10, Name = "Pablo Rodriguez", Specialty = "Neurology",        UserId = -12 },
        ];
    }
}
