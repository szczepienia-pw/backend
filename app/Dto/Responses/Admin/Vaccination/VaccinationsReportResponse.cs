namespace backend.Dto.Responses.Admin.Vaccination
{
	public class VaccinationsReportResponse
	{
		public List<DiseaseReportResponse> Diseases { get; set; }

		public VaccinationsReportResponse(List<DiseaseReportResponse> diseases)
		{
			this.Diseases = diseases;
		}
	}
}
