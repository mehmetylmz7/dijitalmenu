namespace dijitalmenu.Helpers;

public static class PasswordHelper
{
    public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, 12);

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash))
            return false;

        if (storedHash.StartsWith("$2"))
            return BCrypt.Net.BCrypt.Verify(password, storedHash);

        // Legacy plain-text passwords are no longer accepted for security.
        // Users with unhashed passwords must reset their password.
        return false;
    }

    public static bool NeedsRehash(string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash) || !storedHash.StartsWith("$2"))
            return true;

        var parts = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 && int.TryParse(parts[1], out var workFactor) && workFactor < 12)
        {
            return true;
        }

        return false;
    }
}
