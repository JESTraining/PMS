using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class DoctorScheduleByDate
    {
        public int id;
        public string name = string.Empty;
        public int totalDateAppointments;
        public int availableAppointments;
        public int pendingAppointments;
    }
}
