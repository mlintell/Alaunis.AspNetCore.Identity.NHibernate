namespace AiryCore.Identity.Contract
{
    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users login information using NHibernate.
    /// </summary>
    public interface IIdentityUserLogin<TUser>
    {
        /// <summary>
        /// Gets or sets the login provider for the login (e.g. facebook, google)
        /// </summary>
        string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the friendly name used in a UI for this login.
        /// </summary>
        string ProviderDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique provider identifier for this login.
        /// </summary>
        string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this login.
        /// </summary>
        TUser User { get; set; }
    }
}