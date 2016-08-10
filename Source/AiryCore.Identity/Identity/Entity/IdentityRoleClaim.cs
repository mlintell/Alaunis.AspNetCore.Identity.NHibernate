// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace AiryCore.Identity.Entity
{
    using System;
    using System.Security.Claims;

    using AiryCore.Identity.Contract;
    using AiryCore.Identity.Core;

    /// <summary>
    /// The default entity that represents one specific role claim, for roles that have a string based key.
    /// </summary>
    public class IdentityRoleClaim : IdentityRoleClaim<string, IdentityUser>
    {
    }

    /// <summary>
    /// Represents a claim that is granted to all users within a role.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key of the role associated with this claim.</typeparam>
    public class IdentityRoleClaim<TKey> : IdentityRoleClaim<TKey, IdentityRole<TKey>>
        where TKey : IEquatable<TKey>
    {
    }

    /// <summary>
    /// Represents a claim that is granted to all users within a role.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key of the role associated with this claim.</typeparam>
    /// <typeparam name="TRole">The type to use for the Role.</typeparam>
    public class IdentityRoleClaim<TKey, TRole> : EntityWithId<TKey>, IIdentityRoleClaim<TKey, TRole>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public virtual string ClaimValue { get; set; }

        /// <summary>
        /// Gets or sets the role associated with this claim.
        /// </summary>
        public virtual TRole Role { get; set; }

        /// <summary>
        /// Initializes by copying ClaimType and ClaimValue from the other claim.
        /// </summary>
        /// <param name="other">The claim to initialize from.</param>
        public virtual void InitializeFromClaim(Claim other)
        {
            this.ClaimType = other?.Type;
            this.ClaimValue = other?.Value;
        }

        /// <summary>
        /// Constructs a new claim with the type and value.
        /// </summary>
        /// <returns></returns>
        public virtual Claim ToClaim()
        {
            return new Claim(this.ClaimType, this.ClaimValue);
        }
    }
}