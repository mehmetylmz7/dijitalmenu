using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface ICategorySuggestionService
    {
        List<string> GetSuggestions(List<string> existingCategories, string restaurantName);
    }
}
