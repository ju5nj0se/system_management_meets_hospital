using Microsoft.AspNetCore.Mvc;
using SystemManagementMeets.Data;
using SystemManagementMeets.Models;

namespace SystemManagementMeets.Controllers;

public class PatientController : Controller
{
    private readonly AppDbContext _context;
    
    public PatientController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? document)
    {
        var patients = _context.Patients.OrderBy(p => p.Age).AsQueryable();

        if (!string.IsNullOrEmpty(document))
        {
            patients = patients.Where(p => p.Document == document);
        }
        
        ViewData["Title"] = "Patients";
        return View(patients.ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create([Bind("Name, Document, Age, Phone, Email")] Patient patient)
    {
        if (!ModelState.IsValid)
        {
            return View(patient);
        }
        
        bool exist = _context.Patients.Any(x => x.Document == patient.Document);

        if (exist)
        {
            TempData["Message"] = "Patient with that Document already exists";
            return View(patient);
        }

        try
        {
            
            _context.Patients.Add(patient);
            _context.SaveChanges();
            TempData["OK"] = true;
            TempData["Message"] = "Patient Created";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {   
            Console.WriteLine(e.Message);
            TempData["Message"] = "An error occured while creating patient";
            return RedirectToAction(nameof(Index));
        }
    }
    
    public IActionResult Delete(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient is null)
        {
            TempData["Message"] = "Patient not found";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Patients.Remove(patient);
            _context.SaveChanges();
            TempData["OK"] = true;
            TempData["Message"] = "Patient Deleted";
            return RedirectToAction(nameof(Index));
        } 
        catch (Exception e)
        {   
            Console.WriteLine(e.Message);
            TempData["Message"] = "An error occured while deleted patient";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Details(int id)
    {
        var patient = _context.Patients.Find(id);

        if (patient is null)
        {
            TempData["Message"] = "Patient not found";
            return RedirectToAction(nameof(Index));
        }
        
        return View(patient);
    }
    
    public IActionResult Update([Bind("Id, Name, Document, Age, Phone, Email")] Patient patient)
    {
        bool exist = _context.Patients.Any(u => u.Id == patient.Id);
        if (!exist)
        {
            TempData["Message"] = "Patient not found";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Details), patient);
        }
        
        bool exits = _context.Doctors.Where(d => d.Id != patient.Id).Any(d => d.Document == patient.Document);
        if (exits)
        {
            TempData["Message"] = "Patient with that Document already exists";
            return View("Details", patient);
        }
        
        _context.Patients.Update(patient);
        _context.SaveChanges();
        TempData["OK"] = true;
        TempData["Message"] = "Patient updated";
        return RedirectToAction(nameof(Index));
    }
}