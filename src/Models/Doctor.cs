using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SystemManagementMeets.Models;

public class Doctor
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("document")]
    public string Document { get; set; }
    
    [Column("especiality")]
    public string Especiality { get; set; }
    
    [Column("phone")]
    public string Phone { get; set; }
    
    [Column("email")]
    public string Email { get; set; }

    [ValidateNever] public IEnumerable<Meet> MeetsDoctor { get; set; }
}