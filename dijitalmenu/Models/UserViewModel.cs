namespace dijitalmenu.Models;

public class UserViewModel
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public int RestaurantId { get; set; }

    public string? Password { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmPassword { get; set; }
}