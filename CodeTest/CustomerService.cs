namespace CodeTest
{
    using System;
    using System.Linq;

    public class CustomerService
    {
        private readonly IArchiveDataService archiveDataService;
        private readonly IFailoverRepository failoverRepository;
        private readonly IFailoverCustomerDataAccess failoverCustomerDataAccess;
        private readonly bool isFailoverModeEnabled;

        public CustomerService(
            IArchiveDataService archiveDataService,
            IFailoverRepository failoverRepository,
            IFailoverCustomerDataAccess failoverCustomerDataAccess,
            bool isFailoverModeEnabled)
        {
            this.archiveDataService = archiveDataService;
            this.failoverRepository = failoverRepository;
            this.failoverCustomerDataAccess = failoverCustomerDataAccess;
            this.isFailoverModeEnabled = isFailoverModeEnabled;
        }

        public Customer GetCustomer(int customerId, bool isCustomerArchived)
        {
            if (isCustomerArchived)
            {
                return this.archiveDataService.GetArchivedCustomer(customerId);
            }

            CustomerResponse customerResponse = null;
            Customer customer = null;

            if (this.HasExceededMaximumNumberOfFailures())
            {
                customerResponse = this.failoverCustomerDataAccess.GetCustomerById(customerId);
            }
            else
            {
                var dataAccess = new CustomerDataAccess();
                customerResponse = dataAccess.LoadCustomer(customerId);
            }

            if (customerResponse.IsArchived)
            {
                var archivedDataService = new ArchivedDataService();
                customer = archivedDataService.GetArchivedCustomer(customerId);
            }
            else
            {
                customer = customerResponse.Customer;
            }

            return customer;
        }

        private bool HasExceededMaximumNumberOfFailures()
        {
            const int MaximumNumberOfFailovers = 100;
            var failedRequests = this.GetNumberOfFailedRequests();

            return failedRequests > MaximumNumberOfFailovers && this.isFailoverModeEnabled;
        }

        private int GetNumberOfFailedRequests()
        {
            var failoverEntries = this.failoverRepository.GetFailOverEntries();

            return failoverEntries.Count(failoverEntry => failoverEntry.DateTime > DateTime.Now.AddMinutes(-10));
        }
    }
}