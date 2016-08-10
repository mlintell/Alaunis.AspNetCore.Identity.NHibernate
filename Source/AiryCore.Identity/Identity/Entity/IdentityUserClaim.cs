// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace AiryCore.Identity.Entity
{
    using System;
    using System.Security.Claims;

    using AiryCore.Identity.Contract;
    using AiryCore.Identity.Core;

    /// <summary>
    /// The default entity that represents one specific user claim, for users that have a string based key.
    /// </summary>
    public class IdentityUserClaim : IdentityUserClaim<string>
    {
    }

    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for this user that possesses this claim.</typeparam>
    public class IdentityUserClaim<TKey> : IdentityUserClaim<TKey, IdentityUser<TKey>>
        where TKey : IEquatable<TKey>
    {
    }

    /// <summary>
    /// Represents a claim that a user possesses.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for this user that possesses this claim.</typeparam>
    /// <typeparam name="TUser">The type to use for the User.</typeparam>
    public class IdentityUserClaim<TKey, TUser> : EntityWithId<TKey>, IIdentityUserClaim<TKey, TUser>
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
        /// Gets or sets the user associated with this claim.
        /// </summary>
        public virtual TUser User { get; set; }

        /// <summary>
        /// Reads the type and value from the Claim.
        /// </summary>
        /// <param name="claim"></param>
        public virtual void InitializeFromClaim(Claim claim)
        {
            this.ClaimType = claim.Type;
            this.ClaimValue = claim.Value;
        }

        /// <summary>
        /// Converts the entity into a Claim instance.
        /// </summary>
        /// <returns></returns>
        public virtual Claim ToClaim()
        {
            return new Claim(this.ClaimType, this.ClaimValue);
        }
    }
}