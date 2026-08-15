namespace dijitalmenu.Models;

public class RestaurantAccountViewModel
{
    // Profile info
    public string Username { get; set; } = string.Empty;

    // Business info
    public string RestaurantName { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public string? ImportantNotice { get; set; }
    public string? WorkingHours { get; set; }
}

public class ChangePasswordViewModel
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
