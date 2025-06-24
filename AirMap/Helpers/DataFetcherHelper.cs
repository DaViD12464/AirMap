namespace AirMap.Helper
{
    public static class DataFetcherHelper
    {
        public static long? GetUniqueId(this long? id, string test , string test2 = "dasd", string test3 = "dasd")
        {
            var result = AlgorithmsHelper.CuckooSearch();
            var innerResult = CuckooSearch();
            return result;
        }

        public static long? GetUniqueId(this long? id)
        {
            var result = AlgorithmsHelper.CuckooSearch();
            var innerResult = CuckooSearch();
            return result;
        }


        public static long GetUniqueId(this long id)
        {
            return 0;
        }

        public static long? CuckooSearch()
        {
            var v = GetUniqueId(1, test3: "test", test2: "test", test: "23");
            return 0;
        }
    }
}
