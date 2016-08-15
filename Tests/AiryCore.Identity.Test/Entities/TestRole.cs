namespace AiryCore.Identity.Test.Entities
{
    using System;

    using AiryCore.Identity.Entity;

    /// <summary>
    /// Role class for testing AiryCore NHibernate ASP.Net Identity functionality.
    /// </summary>
    public class TestRole : IdentityRole<string, TestRoleClaim, TestUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRole"/> class.
        /// </summary>
        public TestRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRole"/> class.
        /// </summary>
        /// <param name="roleName">The name for the role.</param>
        public TestRole(string roleName)
            : this()
        {
            this.Name = roleName;
        }
    }
}