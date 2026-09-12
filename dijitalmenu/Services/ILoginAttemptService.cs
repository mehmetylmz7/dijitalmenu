using System;

namespace dijitalmenu.Services
{
    public interface ILoginAttemptService
    {
        bool IsLockedOut(string username, string? clientIp = null);
        void RecordFailedAttempt(string username, string? clientIp = null);
        void ResetAttempts(string username);
        int GetFailedAttempts(string username);
        TimeSpan? GetLockoutRemaining(string username);
    }
}
