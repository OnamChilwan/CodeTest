namespace CodeTest
{
    using System.Collections.Generic;


    public interface IFailoverRepository
    {
        List<FailoverEntry> GetFailOverEntries();
    }

    public class FailoverRepository : IFailoverRepository
    {
        public List<FailoverEntry> GetFailOverEntries()
        {
            // return all from fail entries from database
            return new List<FailoverEntry>();
        } 
    }
}