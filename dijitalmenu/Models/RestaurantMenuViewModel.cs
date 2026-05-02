namespace dijitalmenu.Models
{
    public class RestaurantMenuViewModel
    {
        public List<MenuItemVM> MenuItems { get; set; } = new();

        // Kategorileri tanımlı sırayla döndürür
        public List<string> Categories => new[]
        {
            "Mezeler", "Ana Yemekler", "Salatalar", "Tatlılar", "İçecekler"
        }
        .Where(c => MenuItems.Any(m => m.Category == c))
        .ToList();
    }

    public class MenuItemVM
    {
        public string Id          { get; set; } = "";
        public string Name        { get; set; } = "";
        public string Description { get; set; } = "";
        public string Price       { get; set; } = "";
        public string Category    { get; set; } = "";
        public string ImageUrl    { get; set; } = "";
        public bool   IsSpecial   { get; set; }
    }
}
