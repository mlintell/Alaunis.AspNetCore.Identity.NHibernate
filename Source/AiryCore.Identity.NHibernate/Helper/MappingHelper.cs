namespace AiryCore.Helper
{
    using System;
    using System.Collections.Generic;

    using AiryCore.Identity.Contract;
    using AiryCore.Identity.Entity;
    using AiryCore.Identity.NHibernate;

    using NHibernate.Cfg.MappingSchema;
    using NHibernate.Driver;
    using NHibernate.Mapping.ByCode;

    /// <summary>
    /// Helper class to create NHibernate Mappings for the ASP.Net Identity Entities.
    /// </summary>
    /// <typeparam name="TKey">The type used for the unique keys.</typeparam>
    /// <typeparam name="TUser">The User type to map.</typeparam>
    /// <typeparam name="TRole">The Role type to map.</typeparam>
    public class MappingHelper<TKey, TUser, TRole>
        : MappingHelper<TKey, TUser, TKey, IdentityUserClaim<TKey, TUser>, IdentityUserLogin<TUser>, IdentityUserToken<TUser>, TKey, TRole, TKey, IdentityRoleClaim<TKey, TRole>>
        where TKey : IEquatable<TKey>
        where TUser : class, IIdentityUser<TKey, IdentityUserClaim<TKey, TUser>, IdentityUserLogin<TUser>, TRole>
        where TRole : class, IIdentityRole<TKey, IdentityRoleClaim<TKey, TRole>, TUser>
    {
    }

    /// <summary>
    /// Helper class to create NHibernate Mappings for the ASP.Net Identity Entities.
    /// </summary>
    /// <typeparam name="TUserKey">The type used for the User types unique key.</typeparam>
    /// <typeparam name="TUser">The User type to map.</typeparam>
    /// <typeparam name="TUserClaimKey">The type used for the user Claim types unique key.</typeparam>
    /// <typeparam name="TUserClaim">The user Claim type to map.</typeparam>
    /// <typeparam name="TLogin">The Login type to map.</typeparam>
    /// <typeparam name="TToken">The Token type to map.</typeparam>
    /// <typeparam name="TRoleKey">The type used for the Role types unique key.</typeparam>
    /// <typeparam name="TRole">The Role type to map.</typeparam>
    /// <typeparam name="TRoleClaimKey">The type used for the role Claim types unique key.</typeparam>
    /// <typeparam name="TRoleClaim">The role Claim type to map.</typeparam>
    public class MappingHelper<TUserKey, TUser, TUserClaimKey, TUserClaim, TLogin, TToken, TRoleKey, TRole, TRoleClaimKey, TRoleClaim>
        where TUserKey : IEquatable<TUserKey>
        where TUser : class, IIdentityUser<TUserKey, TUserClaim, TLogin, TRole>
        where TUserClaimKey : IEquatable<TUserClaimKey>
        where TUserClaim : class, IIdentityUserClaim<TUserClaimKey, TUser>
        where TLogin : class, IIdentityUserLogin<TUser>
        where TToken : class, IIdentityUserToken<TUser>
        where TRoleKey : IEquatable<TRoleKey>
        where TRole : class, IIdentityRole<TRoleKey, TRoleClaim, TUser>
        where TRoleClaimKey : IEquatable<TRoleClaimKey>
        where TRoleClaim : class, IIdentityRoleClaim<TRoleClaimKey, TRole>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MappingHelper()
        {
            // The default model mapper class:
            //  ** Ignores abstract classes.
            //  ** Uses native generator for int Ids, Generators.GuidComb generator for Guid Ids and string length of 128 for string Ids.
            //  ** Sets a fields size to the properties StringLength attribute if it has one.
            //  ** Sets non-nullable types to be not nullable in the database.
            //  ** Creates indexes based on the index attributes of properties.
            this.Mapper = new DefaultModelMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingHelper{TUserKey, TUser, TUserClaimKey, TUserClaim, TLogin, TToken, TRoleKey, TRole, TRoleClaimKey, TRoleClaim}"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        public MappingHelper(DefaultModelMapper mapper)
        {
            // The default model mapper class:
            //  ** Ignores abstract classes.
            //  ** Uses native generator for int Ids, Generators.GuidComb generator for Guid Ids and string length of 128 for string Ids.
            //  ** Sets a fields size to the properties StringLength attribute if it has one.
            //  ** Sets non-nullable types to be not nullable in the database.
            //  ** Creates indexes based on the index attributes of properties.
            this.Mapper = mapper;
        }

        /// <summary>
        /// The name to use for the Many to Many link table for the User to Roles relationship.
        /// </summary>
        private const string UsersRolesLinkTableName = "AspNetUserRoles";

        /// <summary>
        /// The name to use for the user foreign key on the User to Roles link table.
        /// </summary>
        private const string UsersRolesUserFkName = "FK_AspNetUserRoles_AspNetUsers_UserId";

        /// <summary>
        /// The name to use for the roles foreign key on the User to Roles link table.
        /// </summary>
        private const string UsersRolesRolesFkName = "FK_AspNetUserRoles_AspNetRoles_RoleId";

        /// <summary>
        /// The name to use for the for the User Id foreign keys.
        /// </summary>
        private const string UserIdForeignKeyFieldName = "UserId";

        /// <summary>
        /// The name to use for the for the Role Id foreign keys.
        /// </summary>
        private const string RoleIdForeignKeyFieldName = "RoleId";

        /// <summary>
        /// The model mapper contains the conventions used for mappings.
        /// </summary>
        public DefaultModelMapper Mapper { get; }

        /// <summary>
        /// Gets the mappings to match the EF identity implementation.
        /// </summary>
        /// <returns></returns>
        public HbmMapping GetMappingsToMatchEfIdentity()
        {
            //Get the types to map.
            var entitiesToMap = new List<Type>
            {
                typeof(TUser),
                typeof(TUserClaim),
                typeof(TLogin),
                typeof(TToken),
                typeof(TRole),
                typeof(TRoleClaim)
            };

            // Map the generic User class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            this.Mapper.Class<TUser>(c =>
            {
                c.Table("AspNetUsers");
                c.Property(
                    x => x.UserName,
                    m =>
                    {
                        m.Length(256);
                        m.NotNullable(true);
                        m.UniqueKey("UX_Users_UserName");
                    });
                c.Property(
                    x => x.Email,
                    m =>
                    {
                        m.Length(256);
                    });
                c.Set(
                    x => x.Logins,
                    m =>
                    {
                        m.Key(s =>
                        {
                            s.Column(UserIdForeignKeyFieldName);
                            s.OnDelete(OnDeleteAction.Cascade);
                            s.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId");
                        });
                        m.Inverse(true);
                        m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    });
                c.Set(
                    x => x.Roles,
                    m =>
                    {
                        m.Table(UsersRolesLinkTableName);
                        m.Key(k =>
                        {
                            k.Column(f =>
                            {
                                f.Name(UserIdForeignKeyFieldName);
                                f.NotNullable(true);
                                f.Index("IX_UserRoles_UserId");
                            });
                            k.ForeignKey(UsersRolesUserFkName);
                        });
                        m.Inverse(false);
                    },
                    r =>
                    {
                        r.ManyToMany(p =>
                        {
                            p.Column(RoleIdForeignKeyFieldName);
                            p.ForeignKey(UsersRolesRolesFkName);
                        });
                    });
                c.Bag(
                    x => x.Claims,
                    m =>
                    {
                        m.Key(s =>
                        {
                            s.Column(UserIdForeignKeyFieldName);
                            s.OnDelete(OnDeleteAction.Cascade);
                            s.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId");
                        });
                        m.Inverse(true);
                        m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    },
                    r =>
                    {
                        r.OneToMany();
                    });
            });

            // Map the generic Login class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            this.Mapper.Class<TLogin>(c =>
            {
                c.Table("AspNetUserLogins");
                c.ComposedId(k =>
                {
                    k.Property(
                        x => x.LoginProvider,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.Property(
                        x => x.ProviderKey,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.ManyToOne(x => x.User, m =>
                    {
                        m.Column(UserIdForeignKeyFieldName);
                        m.Index("IX_Logins_UserId");
                    });
                });
            });

            // Map the generic Token class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            this.Mapper.Class<TToken>(c =>
            {
                c.Table("AspNetUserLogins");
                c.ComposedId(k =>
                {
                    k.Property(
                        x => x.LoginProvider,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.Property(
                        x => x.Name,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.Property(
                        x => x.Value,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.ManyToOne(x => x.User, m =>
                    {
                        m.Column(UserIdForeignKeyFieldName);
                        m.Index("IX_Tokens_UserId");
                    });
                });
            });

            // Map the generic Role class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            this.Mapper.Class<TRole>(c =>
            {
                c.Table("AspNetRoles");
                c.Property(
                    x => x.Name,
                    m =>
                    {
                        m.Column(f =>
                        {
                            f.Length(256);
                            f.NotNullable(true);
                            f.Unique(true);
                        });
                    });
                c.Set(
                    x => x.Users,
                    m =>
                    {
                        m.Table(UsersRolesLinkTableName);
                        m.Key(k =>
                        {
                            k.Column(f =>
                            {
                                f.Name(RoleIdForeignKeyFieldName);
                                f.NotNullable(true);
                                f.Index("IX_UserRoles_RoleId");
                            });
                            k.ForeignKey(UsersRolesRolesFkName);
                        });
                        m.Inverse(true);
                    },
                    r =>
                    {
                        r.ManyToMany(p =>
                        {
                            p.Column(UserIdForeignKeyFieldName);
                            p.ForeignKey(UsersRolesUserFkName);
                        });
                    });
                c.Bag(
                    x => x.Claims,
                    m =>
                    {
                        m.Key(s =>
                        {
                            s.Column(RoleIdForeignKeyFieldName);
                            s.OnDelete(OnDeleteAction.Cascade);
                            s.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId");
                        });
                        m.Inverse(true);
                        m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    },
                    r =>
                    {
                        r.OneToMany();
                    });
            });

            // Map the generic user Claim class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            this.Mapper.Class<TUserClaim>(c =>
            {
                c.Table("AspNetUserClaims");
                c.ManyToOne(
                    x => x.User,
                    m =>
                    {
                        m.Column(f =>
                        {
                            f.Name(UserIdForeignKeyFieldName);
                            f.NotNullable(true);
                            f.Index("IX_UserClaims_UserId");
                        });
                    });
                c.Property(
                    x => x.ClaimType,
                    m =>
                    {
                        m.Column(f => f.Length(SqlClientDriver.MaxSizeForLengthLimitedString + 1));
                    });
                c.Property(
                    x => x.ClaimValue,
                    m =>
                    {
                        m.Column(f => f.Length(SqlClientDriver.MaxSizeForLengthLimitedString + 1));
                    });
            });

            // Map the generic role Claim class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            this.Mapper.Class<TRoleClaim>(c =>
            {
                c.Table("AspNetRoleClaims");
                c.ManyToOne(
                    x => x.Role,
                    m =>
                    {
                        m.Column(f =>
                        {
                            f.Name(RoleIdForeignKeyFieldName);
                            f.NotNullable(true);
                            f.Index("IX_RoleClaims_RoleId");
                        });
                    });
                c.Property(
                    x => x.ClaimType,
                    m =>
                    {
                        m.Column(f => f.Length(SqlClientDriver.MaxSizeForLengthLimitedString + 1));
                    });
                c.Property(
                    x => x.ClaimValue,
                    m =>
                    {
                        m.Column(f => f.Length(SqlClientDriver.MaxSizeForLengthLimitedString + 1));
                    });
            });

            return this.Mapper.CompileMappingFor(entitiesToMap);
        }
    }
}