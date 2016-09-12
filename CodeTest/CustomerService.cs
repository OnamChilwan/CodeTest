namespace CodeTest
{
    using System;
    using System.Configuration;

    public class CustomerService
    {
        private readonly IArchiveDataService archiveDataService;

        public CustomerService(IArchiveDataService archiveDataService)
        {
            this.archiveDataService = archiveDataService;
        }

        public Customer GetCustomer(int customerId, bool isCustomerArchived)
        {
            if (isCustomerArchived)
            {
                return this.archiveDataService.GetArchivedCustomer(customerId);
            }
            else
            {
                var failoverRespository = new FailoverRepository();
                var failoverEntries = failoverRespository.GetFailOverEntries();
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

                if (failedRequests > 100 && (ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "true" || ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "True"))
                {
                    customerResponse = FailoverCustomerDataAccess.GetCustomerById(customerId);
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
}
