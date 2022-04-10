using System.Collections.Generic;
using System.Linq;
using backend.Database;
using backend.Dto.Responses.Common.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Vaccines;
using backend.Services.Patient;
using backend_tests.Helpers;
using Moq;
using Xunit;

namespace backend_tests.Unit.Services.Patient
{
    public class VaccinationServiceTest
    {
        private Mock<DataContext> dataContextMock { get; set; }
        private Mock<Mailer> mailerMock { get; set; }
        private VaccinationService vaccinationServiceMock { get; set; }

        public VaccinationServiceTest()
        {
            // Constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.mailerMock = new Mock<Mailer>();
            this.vaccinationServiceMock = new VaccinationService(this.dataContextMock.Object, this.mailerMock.Object);
        }

        [Theory]
        [InlineData("COVID-19", new int[] { 1, 2, 3 })]
        [InlineData("COVID-21", new int[] { 4, 5 })]
        [InlineData("Flu", new int[] { 6, 7 })]
        [InlineData("OTHER", new int[] { 8, 9, 10 })]
        public void TestShowVaccinesForValidDiseases(string diseaseName, int[] expectedIds)
        {
            var response = this.vaccinationServiceMock.ShowAvailableVaccines(DiseaseEnumAdapter.ToEnum(diseaseName));
            Assert.NotNull(response);

            List<VaccineResponse> vaccines = new List<VaccineResponse>(response.Result);
            Assert.Equal(vaccines.Select(vaccine => vaccine.Id).ToArray(), expectedIds);
        }

        [Theory]
        [InlineData("COVID19")]
        [InlineData("COVID21")]
        [InlineData("FLU")]
        [InlineData("Other")]
        [InlineData("RSV")]
        [InlineData("")]
        public void TestShowVaccinesThrowExceptionForInvalidDiseases(string diseaseName)
        {
            Assert.ThrowsAsync<ValidationException>(() => this.vaccinationServiceMock.ShowAvailableVaccines(DiseaseEnumAdapter.ToEnum(diseaseName)));
        }
    }
}

