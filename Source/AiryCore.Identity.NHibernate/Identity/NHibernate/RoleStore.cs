// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace AiryCore.Identity.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using AiryCore.Helper;
    using AiryCore.Identity.Entity;

    using global::NHibernate;
    using global::NHibernate.Linq;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class RoleStore<TRole, TUser> : RoleStore<TRole, TUser, string>
        where TRole : IdentityRole<string, IdentityRoleClaim<string, TRole>, TUser>
        where TUser : IdentityUser<string, IdentityUserClaim<string, TUser>, IdentityUserLogin<TUser>, TRole>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole,TUser}"/>.
        /// </summary>
        /// <param name="context">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(ISession context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }

    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    public class RoleStore<TRole, TUser, TKey> :
        RoleStore
            <TKey, TRole, TKey, TUser, TKey, IdentityRoleClaim<TKey, TRole>, TKey, IdentityUserClaim<TKey, TUser>,
            IdentityUserLogin<TUser>>
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey, IdentityRoleClaim<TKey, TRole>, TUser>
        where TUser : IdentityUser<TKey, IdentityUserClaim<TKey, TUser>, IdentityUserLogin<TUser>, TRole>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole, TUser, TKey}"/>.
        /// </summary>
        /// <param name="context">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(ISession context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }

        /// <summary>
        /// Creates a entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected override IdentityRoleClaim<TKey, TRole> CreateRoleClaim(TRole role, Claim claim)
        {
            return new IdentityRoleClaim<TKey, TRole> { Role = role, ClaimType = claim.Type, ClaimValue = claim.Value };
        }
    }

    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRoleKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TUserKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRoleClaimKey">The type of the primary key for a role claim.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    /// <typeparam name="TUserClaimKey">The type of the primary key for a user claim.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    public abstract class RoleStore<TRoleKey, TRole, TUserKey, TUser, TRoleClaimKey, TRoleClaim, TUserClaimKey,
                                    TUserClaim, TUserLogin> :
                                        IQueryableRoleStore<TRole>,
                                        IRoleClaimStore<TRole>
        where TRoleKey : IEquatable<TRoleKey>
        where TRole : IdentityRole<TRoleKey, TRoleClaim, TUser>
        where TUserKey : IEquatable<TUserKey>
        where TUser : IdentityUser<TUserKey, TUserClaim, TUserLogin, TRole>
        where TRoleClaimKey : IEquatable<TRoleClaimKey>
        where TRoleClaim : IdentityRoleClaim<TRoleClaimKey, TRole>
        where TUserClaimKey : IEquatable<TUserClaimKey>
        where TUserClaim : IdentityUserClaim<TUserClaimKey, TUser>
        where TUserLogin : IdentityUserLogin<TUser>
    {
        private bool _disposed;

        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRoleKey,TRole,TUserKey,TUser,TRoleClaimKey,TRoleClaim,TUserClaimKey,TUserClaim,TUserLogin}"/>.
        /// </summary>
        /// <param name="context">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(ISession context, IdentityErrorDescriber describer = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.Context = context;
            this.ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        public bool AutoSaveChanges { get; set; } = false;

        /// <summary>
        /// Gets the database context for this store.
        /// </summary>
        public ISession Context { get; protected set; }

        /// <summary>
        /// If true will call dispose on the Session during Dispose, false means external code is responsible for disposing the session, default is false.
        /// </summary>
        public bool DisposeContext { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// A navigation property for the roles the store contains.
        /// </summary>
        public virtual IQueryable<TRole> Roles => this.Context.Query<TRole>();

        /// <summary>
        /// Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task AddClaimAsync(
            TRole role,
            Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                if (claim == null)
                {
                    throw new ArgumentNullException(nameof(claim));
                }

                role.Claims.Add(this.CreateRoleClaim(role, claim));
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to a strongly typed key object.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An instance of <typeparamref name="TRoleKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual TRoleKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TRoleKey);
            }

            return (TRoleKey)TypeDescriptor.GetConverter(typeof(TRoleKey)).ConvertFromInvariantString(id);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TRoleKey id)
        {
            if (id.Equals(default(TRoleKey)))
            {
                return null;
            }

            return id.ToString();
        }

        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public virtual async Task<IdentityResult> CreateAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            this.Context.Save(role);
            await this.SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public virtual async Task<IdentityResult> DeleteAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            this.Context.Delete(role);

            try
            {
                await this.SaveChanges(cancellationToken);
            }
            catch (StaleObjectStateException)
            {
                return IdentityResult.Failed(this.ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Dispose this role store.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="id">The role ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual Task<TRole> FindByIdAsync(
            string id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TRole role;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                var roleId = this.ConvertIdFromString(id);

                // Lazy loading is fine here we really only want the Role details, we don't want to be loading all users or users Ids that have the role.
                role = this.Context.Get<TRole>(roleId);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TRole>(ex);
            }

            return Task.FromResult(role);
        }

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual Task<TRole> FindByNameAsync(
            string normalizedName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TRole role;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                role = this.Roles.FirstOrDefault(r => r.NormalizedName == normalizedName);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TRole>(ex);
            }

            return Task.FromResult(role);
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public Task<IList<Claim>> GetClaimsAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<Claim> claims;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                claims =
                    role.Claims
                        .Select(c => c.ToClaim())
                        .ToList();
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<IList<Claim>>(ex);
            }

            return Task.FromResult(claims);
        }

        /// <summary>
        /// Get a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
        public virtual Task<string> GetNormalizedRoleNameAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string name;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                name = role.NormalizedName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(name);
        }

        /// <summary>
        /// Gets the ID for a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            string roleId;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                roleId = this.ConvertIdToString(role.Id);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(roleId);
        }

        /// <summary>
        /// Gets the name of a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
        public Task<string> GetRoleNameAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string name;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                name = role.Name;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(name);
        }

        /// <summary>
        /// Removes the <paramref name="claim"/> given from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove from the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task RemoveClaimAsync(
            TRole role,
            Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                if (claim == null)
                {
                    throw new ArgumentNullException(nameof(claim));
                }

                var claims =
                    role.Claims.Where(
                        rc =>
                        rc.ClaimValue == claim.Value
                        && rc.ClaimType == claim.Type)
                        .ToList();

                foreach (var c in claims)
                {
                    role.Claims.Remove(c);
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be set.</param>
        /// <param name="normalizedName">The normalized name to set</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedRoleNameAsync(
            TRole role,
            string normalizedName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                role.NormalizedName = normalizedName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the name of a role in the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task SetRoleNameAsync(
            TRole role,
            string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }

                role.Name = roleName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public virtual async Task<IdentityResult> UpdateAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Context.Update(role);

            try
            {
                await this.SaveChanges(cancellationToken);
            }
            catch (StaleObjectStateException)
            {
                return IdentityResult.Failed(this.ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Creates a entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected abstract TRoleClaim CreateRoleClaim(TRole role, Claim claim);

        /// <summary>
        /// If disposing, calls dispose on the Context.  Always nulls out the Context.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.DisposeContext)
            {
                this.Context?.Dispose();
            }

            this._disposed = true;
            this.Context = null;
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>Saves the current store.</summary>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        private Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (this.AutoSaveChanges)
                {
                    this.Context.Flush();
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }
    }
}