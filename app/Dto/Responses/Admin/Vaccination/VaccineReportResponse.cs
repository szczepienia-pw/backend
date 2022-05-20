namespace backend.Dto.Responses.Admin.Vaccination
{
	public class VaccineReportResponse
	{
		public string Name { get; set; }
		public int Count { get; set; }

		public VaccineReportResponse(string name, int count)
		{
			this.Name = name;
			this.Count = count;
		}
	}
}
