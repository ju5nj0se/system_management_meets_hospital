using System.ComponentModel.DataAnnotations;


namespace HospitalSanVicente.Validations
{
    public class CompareTimesAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;

        public CompareTimesAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Obtenemos el valor actual (tiempo de inicio)
            var currentValue = value as TimeSpan? ?? default;

            // Buscamos la otra propiedad (tiempo de fin)
            var otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName);

            if (otherProperty == null)
                return new ValidationResult($"La propiedad {_otherPropertyName} no existe.");

            // Obtenemos el valor del tiempo de fin
            var otherValue = otherProperty.GetValue(validationContext.ObjectInstance) as TimeSpan? ?? default;

            // Validamos que el tiempo de inicio NO sea mayor al tiempo de fin
            if (currentValue > otherValue)
            {
                return new ValidationResult(ErrorMessage ?? $"El tiempo de inicio no puede ser mayor que {_otherPropertyName}.");
            }

            return ValidationResult.Success!;
        }
    }
}