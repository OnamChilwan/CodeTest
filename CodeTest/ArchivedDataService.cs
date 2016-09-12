namespace CodeTest
{
    public interface IArchiveDataService
    {
        Customer GetArchivedCustomer(int customerId);
    }

    public class ArchivedDataService : IArchiveDataService
    {
        public Customer GetArchivedCustomer(int customerId)
        {
            // retrieve customer from archive data service
            return new Customer();
        }
    }
}