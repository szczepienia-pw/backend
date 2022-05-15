using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Database;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Visits;
using backend.Services.Admin;
using backend_tests.Helpers;
using Moq;
using Xunit;

namespace backend_tests.Admin
{
    public partial class AdminVaccinationServiceTest
    {
        private readonly Mock<DataContext> dataContextMock;
        private readonly Mock<Mailer> mailerMock;
        private readonly AdminVaccinationService adminVaccinationService;

        public AdminVaccinationServiceTest()
        {
            // Constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.mailerMock = new Mock<Mailer>();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null
            ));

            this.adminVaccinationService = new AdminVaccinationService(this.dataContextMock.Object, this.mailerMock.Object);

            Semaphores.slotSemaphore = new Semaphore(1, 1);
        }

        // Change vaccination slot
        [Theory]
        [InlineData(1, 1)]
        public void UtTestChangeVaccinationSlotForAvailableSlotAndValidVaccinationReturnsSuccess(int vaccinationId, int newSlotId)
        {
            var response = this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId).Result;

            Assert.NotNull(response);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData(1, 1)]
        public void UtTestChangeVaccinationSlotForAvailableSlotAndValidVaccinationChangesSlot(int vaccinationId, int newSlotId)
        {
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));

            VaccinationModel vaccination = this.dataContextMock.Object.Vaccinations.First(vaccination => vaccination.Id == vaccinationId);
            VaccinationSlotModel oldSlot = this.dataContextMock.Object.VaccinationSlots.First(slot => slot.Id == vaccination.VaccinationSlotId);
            VaccinationSlotModel newSlot = this.dataContextMock.Object.VaccinationSlots.First(slot => slot.Id == newSlotId);

            var response = this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId).Result;

            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            Assert.True(newSlot.Reserved);
            Assert.False(oldSlot.Reserved);
            Assert.Equal(newSlot.Id, vaccination.VaccinationSlotId);
            Assert.Equal(newSlot, vaccination.VaccinationSlot);
        }

        [Theory]
        [InlineData(1, 1)]
        public void UtTestChangeVaccinationSlotForAvailableSlotAndValidVaccinationSendEmail(int vaccinationId, int newSlotId)
        {
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));

            VaccinationModel vaccination = this.dataContextMock.Object.Vaccinations.First(vaccination => vaccination.Id == vaccinationId);
            VaccinationSlotModel oldSlot = this.dataContextMock.Object.VaccinationSlots.First(slot => slot.Id == vaccination.VaccinationSlotId);
            VaccinationSlotModel newSlot = this.dataContextMock.Object.VaccinationSlots.First(slot => slot.Id == newSlotId);

            var response = this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId).Result;

            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            this.mailerMock.Verify(mailer => mailer.SendEmailAsync(
                vaccination.Patient.Email,
                "Vaccination visit slot changed",
                It.Is<string>(body => body.Contains(oldSlot.Date.ToShortDateString()) &&
                                      body.Contains(newSlot.Date.ToShortDateString()) &&
                                      body.Contains(vaccination.Vaccine.Disease.ToString()) &&
                                      body.Contains("System Administrator")),
                null
            ), Times.Once);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        public void UtTestChangeVaccinationSlotForReservedSlotAndValidVaccinationThrowsException(int vaccinationId, int newSlotId)
        {
            Assert.ThrowsAsync<ConflictException>(() => this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId));
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        public void UtTestChangeVaccinationSlotForCompletedOrCanceledVaccinationThrowsException(int vaccinationId, int newSlotId)
        {
            Assert.ThrowsAsync<ConflictException>(() => this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(int.MaxValue, 1)]
        [InlineData(int.MinValue, 1)]
        public void UtTestChangeVaccinationSlotForNotValidVaccinationThrowsException(int vaccinationId, int newSlotId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        [InlineData(1, int.MinValue)]
        public void UtTestChangeVaccinationSlotForNotValidVaccinationSlotThrowsException(int vaccinationId, int newSlotId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, newSlotId));
        }
    }
}