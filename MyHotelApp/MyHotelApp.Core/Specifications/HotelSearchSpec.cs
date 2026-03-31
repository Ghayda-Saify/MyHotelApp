using MyHotelApp.Domain.Entities;
using System.Linq.Expressions;

namespace MyHotelApp.Domain.Specifications;

public class HotelSearchSpec
{
    public static Expression<Func<Hotel, bool>> FilterByCityOrName(string query)
    {
        // 1. If search is empty -> return everything
        if (string.IsNullOrWhiteSpace(query))
        {
            return x => true;
        }

        // 2. Normalize to lower case for case-insensitive search
        var lowerQuery = query.ToLower();

        // 3. The Search Logic:
        return x => x.Name.ToLower().Contains(lowerQuery) 
                    || (x.City != null && x.City.Name.ToLower().Contains(lowerQuery));
    }
}