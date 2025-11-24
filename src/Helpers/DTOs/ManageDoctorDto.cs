using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SystemManagementMeets.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaGestionCitasHospital.ViewModels;

public class ManageDoctorDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    public string Document { get; set; }
    
    [RegularExpression("^(pediatric|surgery|cardiology)$", ErrorMessage = "Invalid field")]
    [Required(ErrorMessage = "Field required")]
    public string Especiality { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    public string Phone { get; set; }
            
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Required(ErrorMessage = "Field required")]
    public string Email { get; set; }
}