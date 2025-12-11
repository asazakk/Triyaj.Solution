﻿using Microsoft.EntityFrameworkCore;
using Triyaj.Domain.Entities;

namespace Triyaj.Infrastructure;

public class TriyajDbContext : DbContext
{
    public TriyajDbContext(DbContextOptions<TriyajDbContext> options) : base(options){ }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Encounter> Encounters { get; set; }
    public DbSet<TriageAssesment> TriageAssesments { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<ArrivalSource> ArrivalSources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Patient>().HasIndex(p => p.NationalId).IsUnique();
        modelBuilder.Entity<Encounter>().HasOne(e => e.Patient).WithMany();
        modelBuilder.Entity<TriageAssesment>().HasOne(e => e.Encounter).WithMany();
        
        // Gender ve ArrivalSource için unique index
        modelBuilder.Entity<Gender>().HasIndex(g => g.Code).IsUnique();
        modelBuilder.Entity<ArrivalSource>().HasIndex(a => a.Code).IsUnique();
    }
}
