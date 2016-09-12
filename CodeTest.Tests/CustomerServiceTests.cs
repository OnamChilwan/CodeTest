namespace CodeTest.Tests
{
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CustomerServiceTests
    {
        public class Given_An_Archived_Customer
        {
            private Customer customer;
            private Mock<IArchiveDataService> archiveDataService;

            [SetUp]
            public void When_Requesting_For_Customer_Details()
            {
                this.archiveDataService = new Mock<IArchiveDataService>();
                var subject = new CustomerService(this.archiveDataService.Object);

                this.archiveDataService.Setup(x => x.GetArchivedCustomer(123)).Returns(new Customer { Id = 123, Name = "Test Dummy" });
                this.customer = subject.GetCustomer(123, true);
            }

            [Test]
            public void Then_Customer_Id_Is_Correct()
            {
                Assert.That(this.customer.Id, Is.EqualTo(123));
            }

            [Test]
            public void Then_Customer_Name_Is_Correct()
            {
                Assert.That(this.customer.Name, Is.EqualTo("Test Dummy"));
            }

            [Test]
            public void Then_The_Archive_Data_Service_Is_Called()
            {
                this.archiveDataService.Verify(x => x.GetArchivedCustomer(123), Times.Once);
            }
        }
    }
}