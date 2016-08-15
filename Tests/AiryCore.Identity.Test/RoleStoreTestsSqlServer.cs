namespace AiryCore.Identity.Test
{
    using System;
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// NHibernate persistence tests for ASP.Net Identity RoleStore using Microsoft SQL Server.
    /// </summary>
    [TestFixture]
    public class RoleStoreTestsSqlServer : RoleStoreTests
    {
        /// <summary>
        /// Sets up the test. Creates an NHibernate Session factory and builds a fresh database schema.
        /// </summary>
        [OneTimeSetUp]
        public override void Setup()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string configFile = Path.Combine(baseDir, "hibernate.cfg.mssql.xml");
            SessionFactoryProvider = new SessionFactoryProvider(false, configFile);
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