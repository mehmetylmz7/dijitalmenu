using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Concrete;

public class DefaultCategory
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}
