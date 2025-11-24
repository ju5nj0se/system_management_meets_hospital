using Microsoft.AspNetCore.Mvc;
using HospitalSanVicente.Data;
using HospitalSanVicente.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalSanVicente.ViewModels;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using SistemaGestionCitasHospital.Services;

namespace HospitalSanVicente.Controllers;

public class MeetController : Controller
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public MeetController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }


    public IActionResult Index(string? doctorDocument, string? patientDocument)
    {
        var query = _context.Meets
            .Include(m => m.Patient)
            .Include(m => m.Doctor)
            .AsQueryable();

        if (!string.IsNullOrEmpty(doctorDocument))
        {
            query = query.Where(m => m.Doctor.Document == doctorDocument);
        }

        if (!string.IsNullOrEmpty(patientDocument))
        {
            query = query.Where(m => m.Patient.Document == patientDocument);
        }

        var meets = query.ToList();

        // Guardamos los valores en ViewBag para mantenerlos en el formulario
        ViewBag.DoctorDocument = doctorDocument;
        ViewBag.PatientDocument = patientDocument;
        ViewData["Title"] = "Meets";
        return View(meets);
    }


    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
        return View();
    }

    [HttpPost]
    public IActionResult Create(ManageMeetVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
            return View(vm);
        }

        // Find patient
        var patient = _context.Patients.FirstOrDefault(p => p.Document == vm.Document);
        if (patient == null)
        {
            TempData["Message"] = "Patient with that document not found";
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
            return View(vm);
        }

        // obtain utc to the database
        TimeZoneInfo bogotaZone;
        try
        {
            bogotaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            bogotaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
        }

        if (vm.StartTimeDate >= vm.EndTimeDate)
        {
            ModelState.AddModelError("StartTimeDate", "The start time must be earlier than the end time.");
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
            return View(vm);
        }

        DateTime dateMeetUtc = TimeZoneInfo.ConvertTimeToUtc(vm.DateMeet, bogotaZone);

        bool patientOverlap = _context.Meets.Any(m =>
            m.PatientId == patient.Id &&
            m.DateMeet.Date == dateMeetUtc.Date &&
            (vm.StartTimeDate < m.EndTimeDate && vm.EndTimeDate > m.StartTimeDate)
        );

        if (patientOverlap)
        {
            TempData["Message"] = "The patient already have a meet in this time";
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
            return View(vm);
        }

        bool doctorOverlap = _context.Meets.Any(m =>
            m.DoctorId == vm.DoctorId &&
            m.DateMeet.Date == dateMeetUtc.Date &&
            (vm.StartTimeDate < m.EndTimeDate && vm.EndTimeDate > m.StartTimeDate)
        );

        if (doctorOverlap)
        {
            ModelState.AddModelError(string.Empty, "The doctor already have a meet in this time");
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
            return View(vm);
        }

        var meet = new Meet
        {
            PatientId = patient.Id,
            DoctorId = vm.DoctorId,
            DateMeet = dateMeetUtc,
            StartTimeDate = vm.StartTimeDate,
            EndTimeDate = vm.EndTimeDate,
            Status = vm.Status
        };

        Doctor doctor =  _context.Doctors.First(d => d.Id == vm.DoctorId);

        _context.Meets.Add(meet);
        _context.SaveChanges();

        TempData["Message"] = "Appoinment created";
        TempData["OK"] = true;

        _ = _emailService.SendMeetConfirmationAsync(patient.Email, vm, doctor.Name);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var meet = _context.Meets.Find(id);
        if (meet is null)
        {
            TempData["Message"] = "Meet not found";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Meets.Remove(meet);
            _context.SaveChanges();
            TempData["OK"] = true;
            TempData["Message"] = "Meet Deleted";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            TempData["Message"] = "An error occured while deleted meet";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Details(int id)
    {
        var meet = _context.Meets
            .Include(m => m.Doctor)
            .Include(m => m.Patient)
            .FirstOrDefault(m => m.Id == id);

        if (meet is null)
        {
            TempData["Message"] = "Meet not found";
            return RedirectToAction(nameof(Index));
        }


        var vm = new ManageMeetVM()
        {
            Id = meet.Id,
            Document = meet.Patient.Document,
            DoctorId = meet.DoctorId,
            DateMeet = meet.DateMeet,
            StartTimeDate = meet.StartTimeDate,
            EndTimeDate = meet.EndTimeDate,
            Status = meet.Status
        };
        ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");

        return View(vm);
    }

    [HttpPost]
    public IActionResult Update(ManageMeetVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name", vm.DoctorId);
            return View(nameof(Details), vm);
        }


        var meet = _context.Meets.FirstOrDefault(m => m.Id == vm.Id);
        if (meet == null)
        {
            TempData["Message"] = "Meet not found";
            return RedirectToAction(nameof(Index));
        }

        var patient = _context.Patients.FirstOrDefault(p => p.Document == vm.Document);
        if (patient == null)
        {
            TempData["Message"] = "Patient with that document not found";
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name", vm.DoctorId);
            return View(nameof(Details), vm);
        }

        TimeZoneInfo bogotaZone;
        try
        {
            bogotaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            bogotaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
        }

        if (vm.StartTimeDate >= vm.EndTimeDate)
        {
            ModelState.AddModelError("StartTimeDate", "The start time must be less than the end time.");
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name", vm.DoctorId);
            return View(nameof(Details), vm);
        }

        DateTime dateMeetUtc = TimeZoneInfo.ConvertTimeToUtc(vm.DateMeet, bogotaZone);

        bool patientOverlap = _context.Meets.Where(m => m.Id != vm.Id).Any(m =>
            m.DateMeet.Date == dateMeetUtc.Date &&
            (vm.StartTimeDate < m.EndTimeDate && vm.EndTimeDate > m.StartTimeDate)
        );

        if (patientOverlap)
        {
            TempData["Message"] = "The patient already has a meet in this time";
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name", vm.DoctorId);
            return View(nameof(Details), vm);
        }

        bool doctorOverlap = _context.Meets.Where(m => m.Id != vm.Id).Any(m =>
            m.DateMeet.Date == dateMeetUtc.Date &&
            (vm.StartTimeDate < m.EndTimeDate && vm.EndTimeDate > m.StartTimeDate)
        );

        if (doctorOverlap)
        {
            ModelState.AddModelError(string.Empty, "The doctor already has a meet in this time");
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name", vm.DoctorId);
            return View(nameof(Details), vm);
        }

        meet.PatientId = patient.Id;
        meet.DoctorId = vm.DoctorId;
        meet.DateMeet = dateMeetUtc;
        meet.StartTimeDate = vm.StartTimeDate;
        meet.EndTimeDate = vm.EndTimeDate;
        meet.Status = vm.Status;

        _context.Meets.Update(meet);
        _context.SaveChanges();

        TempData["OK"] = true;
        TempData["Message"] = "Appointment updated successfully";
        return RedirectToAction(nameof(Index));
    }
}