namespace CodeTest.Tests
{
    using System;

    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;

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
                var subject = new CustomerService(this.archiveDataService.Object, null, null, false);

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
        }

        public class Given_A_Non_Archived_Customer_And_Has_Failovers
        {
            private Mock<IFailoverRepository> failoverRepository;
            private Mock<IFailoverCustomerDataAccess> failoverCustomerDataAccess;
            private Customer result;

            [SetUp]
            public void When_Requesting_For_Customer_Details()
            {
                this.failoverRepository = new Mock<IFailoverRepository>();
                this.failoverCustomerDataAccess = new Mock<IFailoverCustomerDataAccess>();

                var subject = new CustomerService(null, this.failoverRepository.Object, this.failoverCustomerDataAccess.Object, true);
                var response = new CustomerResponse
                               {
                                   Customer = new Customer { Id = 123, Name = "Test" },
                                   IsArchived = false
                               };
                var failovers = new List<FailoverEntry>();
                for (var i = 0; i <= 101; i++)
                {
                    failovers.Add(new FailoverEntry { DateTime = DateTime.Now.AddMilliseconds(-20) });
                }
                this.failoverRepository.Setup(x => x.GetFailOverEntries()).Returns(failovers);
                this.failoverCustomerDataAccess.Setup(x => x.GetCustomerById(123)).Returns(response);

                this.result = subject.GetCustomer(123, false);
            }

            [Test]
            public void Then_Customer_Id_Is_Correct()
            {
                Assert.That(this.result.Id, Is.EqualTo(123));
            }

            [Test]
            public void Then_Customer_Name_Is_Correct()
            {
                Assert.That(this.result.Name, Is.EqualTo("Test"));
            }
        }
    }
}