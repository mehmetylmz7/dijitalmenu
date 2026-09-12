using System;
using System.Collections.Concurrent;

namespace dijitalmenu.Services
{
    public class LoginAttemptService : ILoginAttemptService
    {
        private class AttemptRecord
        {
            public int FailCount { get; set; }
            public DateTime LastFailedUtc { get; set; }
            public DateTime? LockoutUntilUtc { get; set; }
        }

        private readonly ConcurrentDictionary<string, AttemptRecord> _records = new(StringComparer.OrdinalIgnoreCase);

        private const int MaxFailedAttemptsBeforeThrottle = 5;
        private static readonly TimeSpan DefaultThrottleDuration = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ExtendedThrottleDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan AttemptSlidingWindow = TimeSpan.FromMinutes(15);

        public bool IsLockedOut(string username, string? clientIp = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            var key = username.Trim().ToLowerInvariant();
            if (_records.TryGetValue(key, out var record))
            {
                if (record.LockoutUntilUtc.HasValue)
                {
                    if (DateTime.UtcNow < record.LockoutUntilUtc.Value)
                    {
                        return true;
                    }

                    // Throttle expired, clear lockout but keep count window
                    record.LockoutUntilUtc = null;
                }
            }

            return false;
        }

        public void RecordFailedAttempt(string username, string? clientIp = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return;

            var key = username.Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;

            _records.AddOrUpdate(
                key,
                _ => new AttemptRecord
                {
                    FailCount = 1,
                    LastFailedUtc = now
                },
                (_, existing) =>
                {
                    // If last failure was longer ago than the sliding window, reset counter
                    if (now - existing.LastFailedUtc > AttemptSlidingWindow)
                    {
                        existing.FailCount = 1;
                    }
                    else
                    {
                        existing.FailCount++;
                    }

                    existing.LastFailedUtc = now;

                    // Progressive temporary throttle (not permanent lockout)
                    if (existing.FailCount >= 8)
                    {
                        existing.LockoutUntilUtc = now.Add(ExtendedThrottleDuration);
                    }
                    else if (existing.FailCount >= MaxFailedAttemptsBeforeThrottle)
                    {
                        existing.LockoutUntilUtc = now.Add(DefaultThrottleDuration);
                    }

                    return existing;
                });
        }

        public void ResetAttempts(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return;
            var key = username.Trim().ToLowerInvariant();
            _records.TryRemove(key, out _);
        }

        public int GetFailedAttempts(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return 0;
            var key = username.Trim().ToLowerInvariant();
            if (_records.TryGetValue(key, out var record))
            {
                if (DateTime.UtcNow - record.LastFailedUtc <= AttemptSlidingWindow)
                {
                    return record.FailCount;
                }
            }
            return 0;
        }

        public TimeSpan? GetLockoutRemaining(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            var key = username.Trim().ToLowerInvariant();
            if (_records.TryGetValue(key, out var record) && record.LockoutUntilUtc.HasValue)
            {
                var remaining = record.LockoutUntilUtc.Value - DateTime.UtcNow;
                if (remaining > TimeSpan.Zero)
                {
                    return remaining;
                }
            }
            return null;
        }
    }
}
