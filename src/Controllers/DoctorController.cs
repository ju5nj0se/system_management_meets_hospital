using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using HospitalSanVicente.Data;
using HospitalSanVicente.Models;
using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasHospital.ViewModels;

namespace HospitalSanVicente.Controllers;

public class DoctorController : Controller
{
    private readonly AppDbContext _context;

    public DoctorController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? param)
    {
        IEnumerable<Doctor> doctors = _context.Doctors.AsQueryable();

        if (!string.IsNullOrEmpty(param))
        {
            doctors = doctors.Where(p => p.Especiality == param);
        }

        ViewData["Title"] = "Doctors";
        var list = doctors.OrderBy(p => p.Id).ToList();
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name, Document, Especiality, Phone, Email")] ManageDoctorDto doctorvm)
    {
        if (!ModelState.IsValid)
        {
            return View(doctorvm);
        }

        try
        {
            bool existDocument = await _context.Doctors.AnyAsync(x => x.Document == doctorvm.Document);
            if (existDocument)
            {
                TempData["Message"] = Messages.Doctor.DocumentAlreadyExists;
                return View(doctorvm);
            }

            bool existEmail = await _context.Doctors.AnyAsync(x => x.Email == doctorvm.Email);
            if (existEmail)
            {
                TempData["Message"] = Messages.Doctor.EmailAlreadyExists;
                return View(doctorvm);
            }
            
            bool existPhone = await _context.Doctors.AnyAsync(x => x.Phone == doctorvm.Phone);
            if (existPhone)
            {
                TempData["Message"] = Messages.Doctor.PhoneAlreadyExists;
                return View(doctorvm);
            }

            Doctor doctor = new Doctor()
            {
                Name = doctorvm.Name,
                Email = doctorvm.Email,
                Document = doctorvm.Document,
                Especiality = doctorvm.Especiality,
                Phone = doctorvm.Phone,
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            TempData["OK"] = true;
            TempData["Message"] = Messages.Doctor.DoctorCreated;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            TempData["Message"] = Messages.Error.UnexpectedError;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool doctorExist = await _context.Doctors.AnyAsync(d => d.Id == id);
            if (!doctorExist)
            {
                TempData["Message"] = Messages.Doctor.NotFound;
                return RedirectToAction(nameof(Index));
            }
            
            Doctor doctor = _context.Doctors.Find(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            TempData["OK"] = true;
            TempData["Message"] = "Doctor Deleted";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            TempData["Message"] = Messages.Error.UnexpectedError;
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Details(int id)
    {
        var user = _context.Doctors.Find(id);
        if (user is null)
        {
            TempData["Message"] = Messages.Doctor.NotFound;
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    public async Task<IActionResult> Update([Bind("Id, Name, Document, Especiality, Phone, Email")] ManageDoctorDto doctorvm)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Details), doctorvm);
        }

        try
        {
            bool exist = await _context.Doctors.AnyAsync(u => u.Id == doctorvm.Id);
            if (!exist)
            {
                TempData["Message"] = Messages.Doctor.NotFound;
                return RedirectToAction(nameof(Index));
            }
            
            bool existDocument = await _context.Doctors.AnyAsync(x => x.Document == doctorvm.Document);
            if (existDocument)
            {
                TempData["Message"] = Messages.Doctor.DocumentAlreadyExists;
                return View(doctorvm);
            }

            bool existEmail = await _context.Doctors.AnyAsync(x => x.Email == doctorvm.Email);
            if (existEmail)
            {
                TempData["Message"] = Messages.Doctor.EmailAlreadyExists;
                return View(doctorvm);
            }
            
            bool existPhone = await _context.Doctors.AnyAsync(x => x.Phone == doctorvm.Phone);
            if (existPhone)
            {
                TempData["Message"] = Messages.Doctor.PhoneAlreadyExists;
                return View(doctorvm);
            }
            Doctor doctor = _context.Doctors.Find(doctorvm.Id);

            _context.Doctors.Update(doctor);
            _context.SaveChanges();
            TempData["OK"] = true;
            TempData["Message"] = Messages.Doctor.DoctorUpdated;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            TempData["Message"] = Messages.Error.UnexpectedError;
            return RedirectToAction(nameof(Index));
        }
    }
}