namespace CodeTest
{

    public interface IFailoverCustomerDataAccess
    {
        CustomerResponse GetCustomerById(int customerId);
    }

    public class FailoverCustomerDataAccess
    {
        public static CustomerResponse GetCustomerById(int id)
        {
            // retrieve customer from database
            return new CustomerResponse();
        }
    }
}