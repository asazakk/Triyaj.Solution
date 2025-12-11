﻿﻿using Triyaj.Domain.Entities;
using Triyaj.Domain.ValueObjects;

namespace Triyaj.Infrastructure.Seed;

public static class DbSeeder
{
    public static void Seed(TriyajDbContext context)
    {
        // Cinsiyet verileri
        if (!context.Genders.Any())
        {
            context.Genders.AddRange(
                new Gender { Id = 1, Code = "M", Name = "Erkek", IsActive = true },
                new Gender { Id = 2, Code = "F", Name = "Kadın", IsActive = true }
            );
            context.SaveChanges();
        }

        // Geliş şekli verileri
        if (!context.ArrivalSources.Any())
        {
            context.ArrivalSources.AddRange(
                new ArrivalSource { Id = 1, Code = "Walk-in", Name = "Yürüyerek", IsActive = true },
                new ArrivalSource { Id = 2, Code = "Ambulance", Name = "Ambulans", IsActive = true },
                new ArrivalSource { Id = 3, Code = "Transfer", Name = "Sevk", IsActive = true },
                new ArrivalSource { Id = 4, Code = "Other", Name = "Diğer", IsActive = true }
            );
            context.SaveChanges();
        }

        // Örnek hasta
        if (!context.Patients.Any())
        {
            var patient = new Patient 
            { 
                Id = Guid.NewGuid(),
                NationalId = "10000000146", // Geçerli TC
                FirstName = "Test", 
                LastName = "Hasta", 
                BirthDate = new DateTime(1990, 1, 1), 
                Gender = "M", 
                Phone = "05001234567" 
            };
            
            context.Patients.Add(patient);
            context.SaveChanges();
            
            // Örnek encounter
            var encounter = new Encounter
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                ArrivalTime = DateTime.Now.AddMinutes(-30),
                Source = "Walk-in",
                TriageLevel = TriageLevel.Undefined,
                Status = "Waiting",
                Queueposition = 1
            };
            
            context.Encounters.Add(encounter);
            context.SaveChanges();
        }
    }
}
