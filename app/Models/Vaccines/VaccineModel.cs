namespace backend.Models.Vaccines
{
    public class VaccineModel : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DiseaseEnum Disease { get; set; }
        public int RequiredDoses { get; set; }
    }
}
