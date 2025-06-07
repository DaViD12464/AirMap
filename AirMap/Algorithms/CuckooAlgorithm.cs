
using Microsoft.IdentityModel.Tokens;

namespace AirMap.Algorithms
{
    public class CuckooAlgorithm<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Entry
        {
            public TKey? Key { get; set; }
            public TValue? Value { get; set; }
            public bool IsOccupied { get; set; }

            public Entry()
            {
                IsOccupied = false;
            }

            public Entry(TKey? key, TValue? value)
            {
                Key = key;
                Value = value;
                IsOccupied = true;
            }
        }
        public long CuckooId()
        {
            

            try
            {
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
       
    }
    
    

}
