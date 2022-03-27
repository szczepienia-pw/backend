using System;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;

namespace backend.Services.Patient
{
	public class PatientService
	{
        private readonly DataContext dataContext;
        private readonly SecurePasswordHasher securePasswordHasher;

        private void ValidatePatient(string? email = null, string? pesel = null)
        {
            if (email != null)
                this.dataContext.Patients.CheckDuplicate(patient => patient.Email == email,
                                                     new ConflictException("Specified e-mail already exists."));

            if (pesel != null)
            {
                this.dataContext.Patients.CheckDuplicate(patient => patient.Pesel == pesel,
                                                     new ConflictException("Specified PESEL already exists."));

                if (!PeselValidator.Validate(pesel))
                    throw new ValidationException("Invalid PESEL.");
            }
        }

        public PatientService(DataContext dataContext, SecurePasswordHasher securePasswordHasher)
        {
            this.dataContext = dataContext;
            this.securePasswordHasher = securePasswordHasher;
        }

        public async Task<PatientResponse> Register(PatientRegistrationRequest request)
        {
            this.ValidatePatient(request.Email, request.Pesel);

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

            return new PatientResponse(patient);
        }

        public async Task<PatientResponse> EditPatient(PatientModel patient, EditPatientRequest request)
        {
            this.ValidatePatient(request.Email, request.Pesel);

            if (request.Address != null)
            {
                var address = request.Address;
                if (address.Street != null)
                    patient.Address.Street = address.Street;

                if (address.City != null)
                    patient.Address.City = address.City;

                if (address.ZipCode != null)
                    patient.Address.ZipCode = address.ZipCode;

                if (address.HouseNumber != null)
                    patient.Address.HouseNumber = address.HouseNumber;

                if (address.LocalNumber != null)
                    patient.Address.LocalNumber = address.LocalNumber;
            }

            if (request.FirstName != null)
                patient.FirstName = request.FirstName;

            if (request.LastName != null)
                patient.LastName = request.LastName;

            if (request.Pesel != null)
                patient.Pesel = request.Pesel;

            if (request.Email != null)
                patient.Email = request.Email;

            if (request.Password != null)
                patient.Password = this.securePasswordHasher.Hash(request.Password);

            this.dataContext.Update(patient);
            this.dataContext.SaveChanges();

            return new PatientResponse(patient);
        }

        public Task<PatientResponse> EditPatient(int patientId, EditPatientRequest request)
        {
            var patient = this.dataContext.Patients.FirstOrThrow((patient) => patient.Id == patientId, new NotFoundException());
            return this.EditPatient(patient, request);
        }
    }
}

