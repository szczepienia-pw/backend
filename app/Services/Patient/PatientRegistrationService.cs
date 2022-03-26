using System;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;

namespace backend.Services.Patient
{
	public class PatientRegistrationService
	{
        private readonly DataContext dataContext;
        private readonly SecurePasswordHasher securePasswordHasher;

        public PatientRegistrationService(DataContext dataContext, SecurePasswordHasher securePasswordHasher)
        {
            this.dataContext = dataContext;
            this.securePasswordHasher = securePasswordHasher;
        }

        public async Task<PatientModel> Register(PatientRegistrationRequest request)
        {
            // Check for duplicated e-mail
            this.dataContext.Patients.CheckDuplicate(patient => patient.Email == request.Email,
                                                     new ConflictException("Specified e-mail already exists."));

            // Check for duplicated PESEL
            this.dataContext.Patients.CheckDuplicate(patient => patient.Pesel == request.Pesel,
                                                     new ConflictException("Specified PESEL already exists."));                

            // Validate PESEL
            if (!PeselValidator.Validate(request.Pesel))
                throw new ValidationException("Invalid PESEL.");

            // Add patient to database
            AddressModel address = new AddressModel()
            {
                City = request.Address.City,
                ZipCode = request.Address.ZipCode,
                Street = request.Address.Street,
                HouseNumber = request.Address.HouseNumber,
                LocalNumber = request.Address.LocalNumber
            };

            PatientModel patient = new PatientModel()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = this.securePasswordHasher.Hash(request.Password),
                Address = address,
                Pesel = request.Pesel
            };

            this.dataContext.Patients.Add(patient);
            this.dataContext.SaveChanges();

            return patient;
        }
    }
}

