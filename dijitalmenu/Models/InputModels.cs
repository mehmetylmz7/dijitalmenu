using EntityLayer.Concrete;
using System.ComponentModel.DataAnnotations;

namespace dijitalmenu.Models
{
    public class MenuItemInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0 ile 999.999,99 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçiniz.")]
        public int CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
    }

    public class MenuInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Restoran seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir restoran seçiniz.")]
        public int RestaurantId { get; set; }
    }

    public class CategoryInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Menü seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir menü seçiniz.")]
        public int MenuId { get; set; }

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
    }

    public class RestaurantInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Restoran adı zorunludur.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Restoran adı 2 ile 100 karakter arasında olmalıdır.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Slug en fazla 100 karakter olabilir.")]
        public string? Slug { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir tema seçiniz.")]
        public int ThemeId { get; set; }

        [StringLength(25, ErrorMessage = "Telefon en fazla 25 karakter olabilir.")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        public string? Address { get; set; }

        [StringLength(1000, ErrorMessage = "Google Maps linki en fazla 1000 karakter olabilir.")]
        public string? GoogleMapsUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Önemli duyuru en fazla 1000 karakter olabilir.")]
        public string? ImportantNotice { get; set; }

        [StringLength(200, ErrorMessage = "Çalışma saatleri en fazla 200 karakter olabilir.")]
        public string? WorkingHours { get; set; }

        [StringLength(200, ErrorMessage = "Instagram linki en fazla 200 karakter olabilir.")]
        public string? InstagramUrl { get; set; }
    }

    public class ThemeInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tema adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Tema adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ana renk zorunludur.")]
        [StringLength(20, ErrorMessage = "Ana renk en fazla 20 karakter olabilir.")]
        public string PrimaryColor { get; set; } = string.Empty;

        [Required(ErrorMessage = "İkincil renk zorunludur.")]
        [StringLength(20, ErrorMessage = "İkincil renk en fazla 20 karakter olabilir.")]
        public string SecondaryColor { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Arka plan rengi en fazla 20 karakter olabilir.")]
        public string BackgroundColor { get; set; } = "#ffffff";

        [StringLength(100, ErrorMessage = "Yazı tipi en fazla 100 karakter olabilir.")]
        public string FontFamily { get; set; } = "Inter, sans-serif";

        public LayoutType Layout { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
