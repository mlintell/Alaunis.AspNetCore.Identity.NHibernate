namespace AiryCore.Identity.Contract
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users information using NHibernate.
    /// </summary>
    public interface IIdentityUser<out TUserKey, TUserClaim, TUserLogin, TRole>
    {
        /// <summary>
        /// Gets or sets the entity's unique identifier
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The ID may be of type <c>string</c>, <c>int</c>, a custom type, etc.
        ///         The setter is protected to allow unit tests to set this property via reflection
        ///         and to allow domain objects more flexibility in setting this for those objects
        ///         with assigned IDs. It's virtual to allow NHibernate-backed objects to be lazily
        ///         loaded.
        ///     </para>
        /// </remarks>
        TUserKey Id { get; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// The roles that this user has.
        /// </summary>
        ICollection<TRole> Roles { get; }

        /// <summary>
        /// The claims that this user has.
        /// </summary>
        ICollection<TUserClaim> Claims { get; }

        /// <summary>
        /// The logins that this user has.
        /// </summary>
        ICollection<TUserLogin> Logins { get; }
    }
}