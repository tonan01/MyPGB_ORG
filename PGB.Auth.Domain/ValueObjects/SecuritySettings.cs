using PGB.BuildingBlocks.Domain.Exceptions;
using PGB.BuildingBlocks.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.ValueObjects
{
    /// <summary>
    /// Security settings value object
    /// Immutable configuration for auth policies
    /// </summary>
    public class SecuritySettings : ValueObject
    {
        #region Properties
        public int MaxFailedAttempts { get; }
        public int LockoutDurationMinutes { get; }
        public int RefreshTokenLifetimeDays { get; }
        public bool RequireEmailVerification { get; }
        #endregion

        #region Constructors
        private SecuritySettings(
            int maxFailedAttempts,
            int lockoutDurationMinutes,
            int refreshTokenLifetimeDays,
            bool requireEmailVerification)
        {
            MaxFailedAttempts = maxFailedAttempts;
            LockoutDurationMinutes = lockoutDurationMinutes;
            RefreshTokenLifetimeDays = refreshTokenLifetimeDays;
            RequireEmailVerification = requireEmailVerification;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Create settings create validation
        /// </summary>
        public static SecuritySettings Create(
            int maxFailedAttempts = 5,
            int lockoutDurationMinutes = 30,
            int refreshTokenLifetimeDays = 7,
            bool requireEmailVerification = false)
        {
            if (maxFailedAttempts <= 0)
                throw new DomainException("Max failed attempts phải > 0");

            if (lockoutDurationMinutes <= 0)
                throw new DomainException("Lockout duration phải > 0");

            if (refreshTokenLifetimeDays <= 0)
                throw new DomainException("Refresh token lifetime phải > 0");

            return new SecuritySettings(
                maxFailedAttempts,
                lockoutDurationMinutes,
                refreshTokenLifetimeDays,
                requireEmailVerification);
        }

        /// <summary>
        /// Default settings for development
        /// </summary>
        public static SecuritySettings Default() => Create();

        /// <summary>
        /// Strict settings for production
        /// </summary>
        public static SecuritySettings Strict() => Create(
            maxFailedAttempts: 3,
            lockoutDurationMinutes: 60,
            refreshTokenLifetimeDays: 1,
            requireEmailVerification: true);
        #endregion

        #region ValueObject Implementation
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return MaxFailedAttempts;
            yield return LockoutDurationMinutes;
            yield return RefreshTokenLifetimeDays;
            yield return RequireEmailVerification;
        }
        #endregion
    }
}
