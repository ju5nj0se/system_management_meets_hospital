using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalSanVicente.Models;

public class Doctor
{
    [Column("id")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("name")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("document")]
    public string Document { get; set; }
    
    [RegularExpression("^(pediatric|surgery|cardiology)$", ErrorMessage = "Invalid field")]
    [Required(ErrorMessage = "Field required")]
    [Column("especiality")]
    public string Especiality { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("phone")]
    public string Phone { get; set; }
            
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Required(ErrorMessage = "Field required")]
    [Column("email")]
    public string Email { get; set; }

    [NotMapped] 
    [ValidateNever]
    public IEnumerable<Meet> MeetsDoctor { get; set; } = null;
}