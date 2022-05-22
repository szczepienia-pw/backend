namespace backend.Dto.Responses.Admin.Vaccination
{
	public class DiseaseReportResponse
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public List<VaccineReportResponse> Vaccines { get; set; }

		public DiseaseReportResponse(string name, int count, List<VaccineReportResponse> vaccines)
		{
			this.Name = name;
			this.Count = count;
			this.Vaccines = vaccines;
		}
	}
}
