using backend.Models.Accounts;

namespace backend.Dto.Responses.Doctor
{
    public class DoctorResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DoctorResponse(DoctorModel doctor)
        {
            this.Id = doctor.Id;
            this.Email = doctor.Email;
            this.FirstName = doctor.FirstName;
            this.LastName = doctor.LastName;
        }
    }
}
