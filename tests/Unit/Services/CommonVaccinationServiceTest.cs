using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses.Common.Vaccination;
using backend.Dto.Responses.Patient.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Vaccines;
using backend.Models.Visits;
using backend.Services;
using backend.Services.Patient;
using backend_tests.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Xunit;

namespace backend_tests.Vaccination
{
    public partial class CommonVaccinationTest
    {
        private readonly Mock<DataContext> dataContextMock;
        private readonly CommonVaccinationService commonVaccinationServiceMock;
        private readonly CommonVaccinationController commonVaccinationController;

        public CommonVaccinationTest()
        {
            // Constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.commonVaccinationServiceMock = new CommonVaccinationService(this.dataContextMock.Object);
            this.commonVaccinationController = new CommonVaccinationController(this.commonVaccinationServiceMock);
        }

        // Get available vaccination slot
        [Fact]
        public void UtTestGetAvailableVaccinationSlots()
        {
            var response = this.commonVaccinationServiceMock.GetAvailableVaccinationSlots();
            Assert.NotNull(response);

            List<AvailableSlotResponse> slots = new List<AvailableSlotResponse>(response.Result);
            Assert.Equal(slots.Select(slot => slot.Id).ToArray(), this.dataContextMock.Object.VaccinationSlots.Where(slot => !slot.Reserved).Select(slot => slot.Id).ToArray());
            Assert.Equal(slots.Select(slot => slot.Date).ToArray(), this.dataContextMock.Object.VaccinationSlots.Where(slot => !slot.Reserved).Select(slot => slot.Date.ToUniversalTime()).ToArray());
        }
    }
}