namespace AiryCore.Identity.Test
{
    using System.Diagnostics;
    using System.IO;

    using AiryCore.Helper;
    using AiryCore.Identity.NHibernate;
    using AiryCore.Identity.Test.Entities;

    using global::NHibernate;
    using global::NHibernate.Cfg;
    using global::NHibernate.Tool.hbm2ddl;

    /// <summary>
    /// NHibernate session factory provider we only ever want one of these :-)
    /// </summary>
    public class SessionFactoryProvider
    {
        private readonly Configuration _configuration;

        /// <summary>
        /// Default constructer, builds a session factory looking in the <c>app.config</c>, <c>web.config</c> or <c>hibernate.cfg.xml</c> for configuration details.
        /// </summary>
        public SessionFactoryProvider()
        {
            this._configuration = new Configuration();
            this._configuration.Configure();
            this.CreateNHibernateSessionFactory();
        }

        /// <summary>
        /// Builds a session factory using the supplied configuration file.
        /// </summary>
        public SessionFactoryProvider(string nHhibernateConfigFile)
        {
            this._configuration = new Configuration();
            this._configuration.Configure(nHhibernateConfigFile);
            this.CreateNHibernateSessionFactory();
        }

        /// <summary>
        /// The NHibernate session factory use to obtain sessions.
        /// </summary>
        public ISessionFactory DefaultSessionFactory;

        /// <summary>
        /// Builds the database schema.
        /// </summary>
        public void BuildSchema()
        {
            // Build the schema.
            var createSchemaSql = new StringWriter();
            var schemaExport = new SchemaExport(this._configuration);

            // Drop the existing schema.
            schemaExport.Drop(true, true);

            // Print the Sql that will be used to build the schema.
            schemaExport.Create(createSchemaSql, false);
            Debug.Print(createSchemaSql.ToString());

            // Create the schema.
            schemaExport.Create(false, true);
        }

        /// <summary>
        /// Creates the NHibernate session factory.
        /// </summary>
        private void CreateNHibernateSessionFactory()
        {
            if (this.DefaultSessionFactory == null)
            {
                // Build and add the mappings for the test domain entities.
                var domainTypes = new[] { typeof(TestAddress), typeof(TestCar) };
                var domainMapper = new DefaultModelMapper();
                this._configuration.AddMapping(domainMapper.CompileMappingFor(domainTypes));

                // Build and add the mappings for ASP.Net Identity entities.
                var mappingHelper = new MappingHelper<string, TestUser, int, TestUserClaim, TestLogin, TestUserToken, string, TestRole, int, TestRoleClaim>();

                // Customise the ASP.Net Identity User mapping before adding the mappings to the configuration.
                mappingHelper.Mapper.Class<TestUser>(u =>
                {
                    u.Bag(x => x.CarsAvailable, c =>
                    {
                        c.Inverse(true);
                    }, r => r.ManyToMany());
                });
                this._configuration.AddMapping(mappingHelper.GetMappingsToMatchEfIdentity());
                this.DefaultSessionFactory = this._configuration.BuildSessionFactory();
            }
        }
    }
}