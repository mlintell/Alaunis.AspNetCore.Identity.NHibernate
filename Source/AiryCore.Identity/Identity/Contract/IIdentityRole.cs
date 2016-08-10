namespace AiryCore.Identity.Contract
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users role information using NHibernate.
    /// </summary>
    public interface IIdentityRole<out TRoleKey, TRoleClaim, TUser>
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
        TRoleKey Id { get; }

        /// <summary>
        /// The claims that this user has.
        /// </summary>
        ICollection<TRoleClaim> Claims { get; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The users that have this role.
        /// </summary>
        ICollection<TUser> Users { get; }
    }
}