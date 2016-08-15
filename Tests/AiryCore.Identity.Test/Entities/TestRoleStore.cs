namespace AiryCore.Identity.Test.Entities
{
    using System.Security.Claims;

    using AiryCore.Identity.NHibernate;

    using global::NHibernate;

    /// <summary>
    /// RoleStore class for testing AiryCore NHibernate ASP.Net Identity functionality.
    /// </summary>
    public class TestRoleStore : RoleStore<string, TestRole, string, TestUser, int, TestRoleClaim, int, TestUserClaim, TestLogin>
    {
        public TestRoleStore(ISession context)
            : base(context)
        {
        }

        protected override TestRoleClaim CreateRoleClaim(TestRole role, Claim claim)
        {
            return new TestRoleClaim
            {
                Role = role,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }
    }
}