// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace AiryCore.Identity.Entity
{
    using System.Diagnostics.CodeAnalysis;

    using AiryCore.Identity.Contract;

    /// <summary>
    /// The default token, for users that have a string based key.
    /// </summary>
    public class IdentityUserToken : IdentityUserToken<IdentityUser>
    {
    }

    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    public class IdentityUserToken<TUser> : IIdentityUserToken<TUser>
    {
        /// <summary>
        /// Stores hash code the first time it is calculated.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The hash code is cached because a requirement of a hash code is that
        ///         it does not change once calculated. For example, if this entity was
        ///         added to a hashed collection when transient and then saved, we need
        ///         the same hash code or else it could get lost because it would no
        ///         longer live in the same bin.
        ///     </para>
        /// </remarks>
        private int? _cachedHashCode;

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the user that the token belongs to.
        /// </summary>
        public virtual TUser User { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            return this.ValueEquals(obj);
        }

        /// <summary>
        /// Compare this object to another object.
        /// Returns true when:
        ///   The two objects are of exactly the same type or one is in the inheritance hierarchy of the other AND all of their properties are equal.
        ///   The two objects are actually both the same object.  (i.e. They point to the same reference.)
        ///   The two objects are both null.
        /// Otherwise we return false.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>true or false as detailed by the rules in the summary.</returns>
        protected bool ValueEquals(object other)
        {
            // If other is null or not an instance of this type then return false.
            if (other == null || !this.GetType().IsInstanceOfType(other))
            {
                return false;
            }

            // If both entities point to the same reference they must be equal.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // OK if we get here then we need to check the values that make up object.
            var obj = other as IdentityUserToken<TUser>;
            if (obj == null)
            {
                return false;
            }
            if (this.LoginProvider == obj.LoginProvider
                && this.Name == obj.Name
                && this.Value == obj.Value
                && this.User.Equals(obj.User))
            {
                return true;
            }

            return false;
        }

        /// <summary>Serves as a hash function for this type. </summary>
        /// <returns>A hash code for the current object.</returns>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Thanks R#, but In this case this is the only place we set it and we only set it once.")]
        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses", Justification = "Makes the code more readable to have them.")]
        public override int GetHashCode()
        {
            if (this._cachedHashCode != null)
            {
                return this._cachedHashCode.Value;
            }

            // Ref: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // Overflow is fine just wrap.
            unchecked
            {
                const int PrimeToMinimiseCollisions = 486187739;

                int loginProviderHash = this.LoginProvider?.GetHashCode() ?? 0;
                int nameHash = this.Name?.GetHashCode() ?? 0;
                int valueHash = this.Value?.GetHashCode() ?? 0;
                int userHash = this.User == null ? 0 : this.User.GetHashCode();
                int hash = 17;

                hash = (hash * PrimeToMinimiseCollisions) + loginProviderHash;
                hash = (hash * PrimeToMinimiseCollisions) + nameHash;
                hash = (hash * PrimeToMinimiseCollisions) + valueHash;
                hash = (hash * PrimeToMinimiseCollisions) + userHash;

                this._cachedHashCode = hash;

                return this._cachedHashCode.Value;
            }
        }
    }
}