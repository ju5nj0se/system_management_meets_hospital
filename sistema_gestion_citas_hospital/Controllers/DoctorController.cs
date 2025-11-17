using Microsoft.AspNetCore.Mvc;
using HospitalSanVicente.Data;
using HospitalSanVicente.Models;

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
        var doctors  = _context.Doctors.AsQueryable();

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
    public IActionResult Create([Bind("Name, Document, Especiality, Phone, Email")] Doctor doctor)
    {
        if (!ModelState.IsValid)
        {
            return View(doctor);
        }
        
        bool exist = _context.Doctors.Any(x => x.Document == doctor.Document);

        if (exist)
        {
            TempData["Message"] = "Doctor with that Document already exists";
            return View(doctor);
        }

        try
        {
            
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            TempData["OK"] = true;
            TempData["Message"] = "Doctor Created";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {   
            Console.WriteLine(e.Message);
            TempData["Message"] = "An error occured while creating doctor";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Delete(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor is null)
        {
            TempData["Message"] = "Doctor not found";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Doctors.Remove(doctor);
            _context.SaveChanges();
            TempData["OK"] = true;
            TempData["Message"] = "Doctor Deleted";
            return RedirectToAction(nameof(Index));
        } 
        catch (Exception e)
        {   
            Console.WriteLine(e.Message);
            TempData["Message"] = "An error occured while deleted doctor";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Details(int id)
    {
        var user = _context.Doctors.Find(id);

        if (user is null)
        {
            TempData["Message"] = "Doctor not found";
            return RedirectToAction(nameof(Index));
        }
        
        return View(user);
    }

    public IActionResult Update([Bind("Id, Name, Document, Especiality, Phone, Email")] Doctor doctor)
    {
        bool exist = _context.Doctors.Any(u => u.Id == doctor.Id);
        if (!exist)
        {
            TempData["Message"] = "User not found";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Details), doctor);
        }
        
        bool exits = _context.Doctors.Where(d => d.Id != doctor.Id).Any(d => d.Document == doctor.Document);
        if (exits)
        {
            TempData["Message"] = "Doctor with that Document already exists";
            return View("Details", doctor);
        }
        
        _context.Doctors.Update(doctor);
        _context.SaveChanges();
        TempData["ok"] = true;
        TempData["Message"] = "Doctor updated";
        return RedirectToAction(nameof(Index));
    }
}