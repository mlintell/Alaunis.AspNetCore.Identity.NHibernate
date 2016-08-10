namespace AiryCore.Identity.Contract
{
    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users token information using NHibernate.
    /// </summary>
    public interface IIdentityUserToken<TUser>
    {
        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the user that the token belongs to.
        /// </summary>
        TUser User { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        string Value { get; set; }
    }
}