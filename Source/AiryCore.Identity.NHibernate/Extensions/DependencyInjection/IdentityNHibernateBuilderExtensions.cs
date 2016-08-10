// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace AiryCore.Extensions.DependencyInjection
{
    using System;

    using AiryCore.Identity.NHibernate;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class IdentityNHibernateBuilderExtensions
    {
        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddNHibernateStores(this IdentityBuilder builder)
        {
            builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType));
            return builder;
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key used for the users and roles.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddNHibernateStores<TKey>(this IdentityBuilder builder)
            where TKey : IEquatable<TKey>
        {
            builder.Services.TryAdd(
                GetDefaultServices(builder.UserType, builder.RoleType, typeof(TKey)));
            return builder;
        }

        private static IServiceCollection GetDefaultServices(
            Type userType,
            Type roleType,
            Type keyType = null)
        {
            keyType = keyType ?? typeof(string);
            var userStoreType = typeof(UserStore<,,>).MakeGenericType(userType, roleType, keyType);
            var roleStoreType = typeof(RoleStore<,>).MakeGenericType(roleType, keyType);

            var services = new ServiceCollection();

            services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);

            services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStoreType);

            return services;
        }
    }
}