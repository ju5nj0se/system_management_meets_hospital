using Microsoft.EntityFrameworkCore;
using Npgsql;
using HospitalSanVicente.Models;

namespace HospitalSanVicente.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Meet> Meets { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure tables with models
        
        modelBuilder.Entity<Doctor>()
            .ToTable("doctors")
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Patient>()
            .ToTable("patients")
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Meet>()
            .ToTable("meets")
            .HasKey(x => x.Id);
        
        
        //Relations between the tables 
        modelBuilder.Entity<Meet>()
            .HasOne(r => r.Doctor)
            .WithMany(u => u.MeetsDoctor)
            .HasForeignKey(r => r.DoctorId);
        
        modelBuilder.Entity<Meet>()
            .HasOne(r => r.Patient)
            .WithMany(l => l.MeetsPatient)
            .HasForeignKey(r => r.PatientId);
    }
}