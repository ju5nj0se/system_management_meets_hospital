namespace HospitalSanVicente.Models;

public class Messages
{
    public class Doctor
    {
        public const string NotFound = "Doctor not  found";
        public const string DocumentAlreadyExists = "A doctor with this document already exists";
        public const string EmailAlreadyExists = "A doctor with this email already exists";
        public const string PhoneAlreadyExists = "A doctor with this phone already exists";
        public const string DoctorCreated = "Doctor created";
        public const string DoctorUpdated = "Doctor updated";
    }

    public class Error
    {
        public const string UnexpectedError = "An error occured, try again later";
    }
}