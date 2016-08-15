namespace AiryCore.Identity.Test.Entities
{
    using AiryCore.Identity.Entity;

    /// <summary>
    /// User claim class for testing AiryCore NHibernate ASP.Net Identity functionality.
    /// </summary>
    public class TestRoleClaim : IdentityRoleClaim<int, TestRole>
    {
    }
}