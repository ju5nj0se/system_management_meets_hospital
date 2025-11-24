using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using HospitalSanVicente.Validations;

namespace HospitalSanVicente.Models;

public class Meet
{
    [Required(ErrorMessage = "Field required")]
    [Column("id")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("patient_id")]
    public int PatientId { get; set; }
    
    [ForeignKey("PatientId")]
    [ValidateNever]
    public Patient Patient { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("doctor_id")]
    public int DoctorId { get; set; }
    
    [ForeignKey("DoctorId")]
    [ValidateNever]
    public Doctor Doctor { get; set; }
    
    [Required(ErrorMessage = "Field required")]
    [Column("date_meet")]
    public DateTime DateMeet { get; set; }
    
    [CompareTimes("EndTimeDate", ErrorMessage = "The time must be greater than to the end time")]

    [Required(ErrorMessage = "Field required")]
    [Column("time_start")]
    public TimeOnly StartTimeDate { get; set; }
    
    [Required(ErrorMessage = "Field required")] 
    [Column("time_end")]
    public TimeOnly EndTimeDate { get; set; }
    
    [RegularExpression("^(attended|canceled|pending)$", ErrorMessage = "Invalid field")]
    [Column("status")]
    public string Status { get; set; }
}