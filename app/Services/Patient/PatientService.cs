using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses;
using backend.Dto.Responses.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using Microsoft.Extensions.Options;

namespace backend.Services.Patient
{
	public class PatientService
	{
        private readonly DataContext dataContext;
        private readonly SecurePasswordHasher securePasswordHasher;
        private readonly Mailer mailer;
        private readonly FrontendUrlsSettings frontendUrlsSettings;

        // Exposed for UTs
        public void ValidatePatient(string? email = null, string? pesel = null)
        {
            if (email != null)
            {
                this.dataContext.Patients.CheckDuplicate(patient => patient.Email == email,
                                     new ConflictException("Specified e-mail already exists."));

                if (!Validator.ValidateEmail(email))
                    throw new ValidationException("Invalid e-mail.");
            }

            if (pesel != null)
            {
                this.dataContext.Patients.CheckDuplicate(patient => patient.Pesel == pesel,
                                                     new ConflictException("Specified PESEL already exists."));

                if (!Validator.ValidatePesel(pesel))
                    throw new ValidationException("Invalid PESEL.");
            }
        }

        public PatientService(DataContext dataContext, SecurePasswordHasher securePasswordHasher, Mailer mailer, IOptions<FrontendUrlsSettings> frontendUrlsSettings)
        {
            this.dataContext = dataContext;
            this.securePasswordHasher = securePasswordHasher;
            this.mailer = mailer;
            this.frontendUrlsSettings = frontendUrlsSettings.Value;
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

            var verificationToken = Guid.NewGuid().ToString();
            
            PatientModel patient = new PatientModel()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = this.securePasswordHasher.Hash(request.Password),
                Address = address,
                Pesel = request.Pesel,
                VerificationToken = verificationToken
            };

            this.dataContext.Patients.Add(patient);
            this.dataContext.SaveChanges();

            var confirmUrl = this.frontendUrlsSettings.ConfirmRegistration.Replace("{token}", verificationToken);
            _ = this.mailer.SendEmailAsync(
                request.Email,
                "Verify your email",
                $"Your account has been created. Please click link below to verify your email address. <br> <a href='{confirmUrl}'>LINK</a> <br> <br> <small>If you have trouble clicking the link, please type this into your searchbar: {confirmUrl}</small>"
            );

            return new PatientResponse(patient);
        }

        public async Task<SuccessResponse> ConfirmRegistration(ConfirmRegistrationRequest request)
        {
            var patient = this.dataContext.Patients
                .FirstOrDefault(patient => patient.VerificationToken == request.Token);

            if (patient == null) throw new UnauthorizedException("Provided token is not valid");

            patient.VerificationToken = null;
            this.dataContext.Update(patient);
            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }

        public async Task<PatientResponse> EditPatient(PatientModel patient, PatientRequest request)
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

        public Task<PatientResponse> EditPatient(int patientId, PatientRequest request)
        {
            var patient = this.dataContext.Patients.FirstOrThrow((patient) => patient.Id == patientId, new NotFoundException());
            return this.EditPatient(patient, request);
        }
    } 
}

