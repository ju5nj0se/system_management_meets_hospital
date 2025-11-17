using System;
using System.ComponentModel.DataAnnotations;
using HospitalSanVicente.Validations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalSanVicente.ViewModels
{
    public class AdminMeetVM
    {
        [ValidateNever]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Field required")]
        public string Document { get; set; }
        
        [Required(ErrorMessage = "Field required")]
        public int DoctorId { get; set; }
        
        [Required(ErrorMessage = "Field required")]
        [DataType(DataType.Date)]
        public DateTime DateMeet { get; set; }
        
        [Required(ErrorMessage = "Field required")]
        [DataType(DataType.Time)]
        [CompareTimes("EndTimeDate", ErrorMessage = "The start time must be less than the end time.")]
        public TimeOnly StartTimeDate { get; set; }
        
        [Required(ErrorMessage = "Field required")]
        [DataType(DataType.Time)]
        public TimeOnly EndTimeDate { get; set; }
        
        [RegularExpression("^(attended|canceled|pending)$", ErrorMessage = "Invalid status")]
        public string Status { get; set; } = "pending";
    }
}