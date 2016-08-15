namespace AiryCore.Identity.Test
{
    using NUnit.Framework;

    /// <summary>
    /// NHibernate persistence tests for ASP.Net Identity UserStore using Microsoft SQL Server.
    /// </summary>
    [TestFixture]
    public class UserStoreTestsSqlServer : UserStoreTests
    {
        /// <summary>
        /// Sets up the test. Creates an NHibernate Session factory and builds a fresh database schema.
        /// </summary>
        [OneTimeSetUp]
        public override void Setup()
        {
            SessionFactoryProvider = new SessionFactoryProvider("hibernate.cfg.mssql.xml");
            SessionFactory = SessionFactoryProvider.DefaultSessionFactory;
            SessionFactoryProvider.BuildSchema();
        }

        /// <summary>
        /// Disposes of the objects once the test has completed.
        /// </summary>
        [OneTimeTearDown]
        public override void TearDown()
        {
            SessionFactory.Dispose();
            SessionFactory = null;
            SessionFactoryProvider = null;
        }
    }
}