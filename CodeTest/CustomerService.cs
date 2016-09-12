namespace CodeTest
{
    using System;
    using System.Configuration;

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

            var failoverEntries = this.failoverRepository.GetFailOverEntries();
            var failedRequests = 0;

            foreach (var failoverEntry in failoverEntries)
            {
                if (failoverEntry.DateTime > DateTime.Now.AddMinutes(-10))
                {
                    failedRequests++;
                }
            }

            CustomerResponse customerResponse = null;
            Customer customer = null;

            if (failedRequests > 100 && this.isFailoverModeEnabled)
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
    }
}