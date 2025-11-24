using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalSanVicente.Models;

public class Patient
{
    [Column("id")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("name")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("document")]
    public string Document { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Range(18, int.MaxValue, ErrorMessage = "Age must be greater than 18")]
    [Column("age")]
    public int Age { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("phone")]
    public string Phone { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Required(ErrorMessage = "Field required")]
    [Column("email")]
    public string Email { get; set; }

    [NotMapped] 
    [ValidateNever]
    public IEnumerable<Meet> MeetsPatient { get; set; } = null;

}