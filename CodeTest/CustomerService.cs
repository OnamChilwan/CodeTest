namespace CodeTest
{
    using System;
    using System.Configuration;

    public class CustomerService
    {
        public Customer GetCustomer(int customerId, bool isCustomerArchived)
        {

            Customer archivedCustomer = null;

            if (isCustomerArchived)
            {
                var archivedDataService = new ArchivedDataService();
                archivedCustomer = archivedDataService.GetArchivedCustomer(customerId);

                return archivedCustomer;
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
