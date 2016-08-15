namespace AiryCore.Identity.Test.Entities
{
    using System.Security.Claims;

    using AiryCore.Identity.NHibernate;

    using Microsoft.AspNetCore.Identity;

    using global::NHibernate;

    /// <summary>
    /// UserStore class for testing AiryCore NHibernate ASP.Net Identity functionality.
    /// </summary>
    public class TestUserStore : UserStore<string, TestUser, string, TestRole, int, TestUserClaim, int, TestRoleClaim, TestLogin, TestUserToken>
    {
        public TestUserStore(ISession context)
            : base(context)
        {
        }

        protected override TestUserClaim CreateUserClaim(TestUser user, Claim claim)
        {
            var userClaim = new TestUserClaim { User = user };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override TestLogin CreateUserLogin(TestUser user, UserLoginInfo login)
        {
            return new TestLogin
            {
                User = user,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override TestUserToken CreateUserToken(TestUser user, string loginProvider, string name, string value)
        {
            return new TestUserToken
            {
                User = user,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }
    }
}