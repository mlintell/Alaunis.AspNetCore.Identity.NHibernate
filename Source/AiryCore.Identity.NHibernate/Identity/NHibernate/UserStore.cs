// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace AiryCore.Identity.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using AiryCore.Helper;
    using AiryCore.Identity.Entity;

    using global::NHibernate;
    using global::NHibernate.Linq;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public class UserStore<TUser, TRole> : UserStore<TUser, TRole, string>
        where TUser : IdentityUser<string, IdentityUserClaim<string, TUser>, IdentityUserLogin<TUser>, TRole>
        where TRole : IdentityRole<string, IdentityRoleClaim<string, TRole>, TUser>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserStore{TUser, TRole, TContext}"/>.
        /// </summary>
        /// <param name="context">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserStore(ISession context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    public class UserStore<TUser, TRole, TKey> :
        UserStore
            <TKey, TUser, TKey, TRole, TKey, IdentityUserClaim<TKey, TUser>, TKey, IdentityRoleClaim<TKey, TRole>,
            IdentityUserLogin<TUser>,
            IdentityUserToken<TUser>>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey, IdentityUserClaim<TKey, TUser>, IdentityUserLogin<TUser>, TRole>
        where TRole : IdentityRole<TKey, IdentityRoleClaim<TKey, TRole>, TUser>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserStore{TUser, TRole, TKey}"/>.
        /// </summary>
        /// <param name="context">The <see cref="ISession"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserStore(ISession context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserClaim{TKey, TUser}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns></returns>
        protected override IdentityUserClaim<TKey, TUser> CreateUserClaim(TUser user, Claim claim)
        {
            var userClaim = new IdentityUserClaim<TKey, TUser> { User = user };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserLogin{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="login">The sasociated login.</param>
        /// <returns></returns>
        protected override IdentityUserLogin<TUser> CreateUserLogin(TUser user, UserLoginInfo login)
        {
            return new IdentityUserLogin<TUser>
            {
                User = user,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserToken{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="loginProvider">The associated login provider.</param>
        /// <param name="name">The name of the user token.</param>
        /// <param name="value">The value of the user token.</param>
        /// <returns></returns>
        protected override IdentityUserToken<TUser> CreateUserToken(
            TUser user,
            string loginProvider,
            string name,
            string value)
        {
            return new IdentityUserToken<TUser>
            {
                User = user,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUserKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRoleKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserClaimKey">The type of the primary key for a user claim.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
    /// <typeparam name="TRoleClaimKey">The type of the primary key for a role claim.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public abstract class UserStore<TUserKey, TUser, TRoleKey, TRole, TUserClaimKey, TUserClaim, TRoleClaimKey,
                                    TRoleClaim, TUserLogin, TUserToken> :
                                        IUserLoginStore<TUser>,
                                        IUserRoleStore<TUser>,
                                        IUserClaimStore<TUser>,
                                        IUserPasswordStore<TUser>,
                                        IUserSecurityStampStore<TUser>,
                                        IUserEmailStore<TUser>,
                                        IUserLockoutStore<TUser>,
                                        IUserPhoneNumberStore<TUser>,
                                        IQueryableUserStore<TUser>,
                                        IUserTwoFactorStore<TUser>,
                                        IUserAuthenticationTokenStore<TUser>
        where TUserKey : IEquatable<TUserKey>
        where TUser : IdentityUser<TUserKey, TUserClaim, TUserLogin, TRole>
        where TRoleKey : IEquatable<TRoleKey>
        where TRole : IdentityRole<TRoleKey, TRoleClaim, TUser>
        where TUserClaimKey : IEquatable<TUserClaimKey>
        where TUserClaim : IdentityUserClaim<TUserClaimKey, TUser>
        where TRoleClaimKey : IEquatable<TRoleClaimKey>
        where TRoleClaim : IdentityRoleClaim<TRoleClaimKey, TRole>
        where TUserLogin : IdentityUserLogin<TUser>
        where TUserToken : IdentityUserToken<TUser>
    {
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of <see cref="UserStore{TUserKey,TUser,TRoleKey,TRole,TUserClaimKey,TUserClaim,TRoleClaimKey,TRoleClaim,TUserLogin,TUserToken}"/>.
        /// </summary>
        /// <param name="context">The context used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserStore(ISession context, IdentityErrorDescriber describer = null)
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
        /// A navigation property for the users the store contains.
        /// </summary>
        public virtual IQueryable<TUser> Users => this.Context.Query<TUser>();

        private IQueryable<TRole> Roles => this.Context.Query<TRole>();

        private IQueryable<TUserClaim> UserClaims => this.Context.Query<TUserClaim>();

        private IQueryable<TUserLogin> UserLogins => this.Context.Query<TUserLogin>();

        private IQueryable<TUserToken> UserTokens => this.Context.Query<TUserToken>();

        /// <summary>
        /// Adds the <paramref name="claims"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The claim to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task AddClaimsAsync(
            TUser user,
            IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (claims == null)
                {
                    throw new ArgumentNullException(nameof(claims));
                }

                foreach (var claim in claims)
                {
                    user.Claims.Add(this.CreateUserClaim(user, claim));
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Adds the <paramref name="login"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The login to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task AddLoginAsync(
            TUser user,
            UserLoginInfo login,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (login == null)
                {
                    throw new ArgumentNullException(nameof(login));
                }

                user.Logins.Add(this.CreateUserLogin(user, login));
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Adds the given <paramref name="normalizedRoleName"/> to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the role to.</param>
        /// <param name="normalizedRoleName">The role to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task AddToRoleAsync(
            TUser user,
            string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (string.IsNullOrWhiteSpace(normalizedRoleName))
                {
                    throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
                }

                var role =
                    this.Roles.SingleOrDefault(r => r.NormalizedName == normalizedRoleName);

                if (role == null)
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, Resources.RoleNotFound, normalizedRoleName));
                }

                user.Roles.Add(role);
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
        /// <returns>An instance of <typeparamref name="TUserKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual TUserKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TUserKey);
            }

            return (TUserKey)TypeDescriptor.GetConverter(typeof(TUserKey)).ConvertFromInvariantString(id);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TUserKey id)
        {
            if (Equals(id, default(TUserKey)))
            {
                return null;
            }

            return id.ToString();
        }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public virtual async Task<IdentityResult> CreateAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.Context.Save(user);

            await this.SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Deletes the specified <paramref name="user"/> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public virtual async Task<IdentityResult> DeleteAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.Context.Delete(user);

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
        /// Dispose the store
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public virtual Task<TUser> FindByEmailAsync(
            string normalizedEmail,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<TUser> task;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                task = this.GetUserAggregateAsync(u => u.NormalizedEmail == normalizedEmail);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TUser>(ex);
            }

            return task;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByIdAsync(
            string userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<TUser> task;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                TUserKey id = this.ConvertIdFromString(userId);

                // Get method, this allows NHibernate to use its cache when it can.
                //task = this.GetUserAggregateByIdAsync(id);

                // Query method would always hit the database, however...
                // It does not work when we have a string as the generic type for the user id, NHibernate throws a System.NotSupportedException : Boolean Equals(typeof(TUserKey)).
                // Related details in this reference: http://www.primordialcode.com/blog/post/linq-to-nhibernate-string.equals-with-stringcomparison-option/
                // Also == can not be used as the TUserKey generic type could be a struct and structs don't support == by default.
                task = this.GetUserAggregateAsync(u => u.Id.Equals(id));
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TUser>(ex);
            }

            return task;
        }

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key..
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        public virtual Task<TUser> FindByLoginAsync(
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<TUser> task;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                var userLogin = this.UserLogins.FirstOrDefault(
                    l => l.LoginProvider == loginProvider
                    && l.ProviderKey == providerKey);

                //task = userLogin != null
                //           ? this.GetUserAggregateByIdAsync(userLogin.User.Id)
                //           : Task.FromResult<TUser>(null);

                task = userLogin != null
                           ? this.GetUserAggregateAsync(u => u.Id.Equals(userLogin.User.Id))
                           : Task.FromResult<TUser>(null);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TUser>(ex);
            }

            return task;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByNameAsync(
            string normalizedUserName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<TUser> task;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                task = this.GetUserAggregateAsync(u => u.NormalizedUserName == normalizedUserName);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TUser>(ex);
            }

            return task;
        }

        /// <summary>
        /// Retrieves the current failed access count for the specified <paramref name="user"/>..
        /// </summary>
        /// <param name="user">The user whose failed access count should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the failed access count.</returns>
        public virtual Task<int> GetAccessFailedCountAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int count;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                count = user.AccessFailedCount;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(count);
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public virtual Task<IList<Claim>> GetClaimsAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<Claim> claims;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                claims =
                    user.Claims
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
        /// Gets the email address for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetEmailAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string email;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                email = user.Email;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(email);
        }

        /// <summary>
        /// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
        /// false.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
        /// has been confirmed or not.
        /// </returns>
        public virtual Task<bool> GetEmailConfirmedAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool confirmed;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                confirmed = user.EmailConfirmed;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<bool>(ex);
            }

            return Task.FromResult(confirmed);
        }

        /// <summary>
        /// Retrieves a flag indicating whether user lockout can enabled for the specified user.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
        /// </returns>
        public virtual Task<bool> GetLockoutEnabledAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool enabled;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                enabled = user.LockoutEnabled;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<bool>(ex);
            }

            return Task.FromResult(enabled);
        }

        /// <summary>
        /// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any.
        /// Any time in the past should be indicates a user is not locked out.
        /// </summary>
        /// <param name="user">The user whose lockout date should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a <see cref="DateTimeOffset"/> containing the last time
        /// a user's lockout expired, if any.
        /// </returns>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            DateTimeOffset? date;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                date = user.LockoutEnd;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<DateTimeOffset?>(ex);
            }

            return Task.FromResult(date);
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<UserLoginInfo> logins;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                logins =
                    user.Logins
                        .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName))
                        .ToList();
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<IList<UserLoginInfo>>(ex);
            }

            return Task.FromResult(logins);
        }

        /// <summary>
        /// Returns the normalized email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
        /// </returns>
        public virtual Task<string> GetNormalizedEmailAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string email;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                email = user.NormalizedEmail;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(email);
        }

        /// <summary>
        /// Gets the normalized user name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetNormalizedUserNameAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string name;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                name = user.NormalizedUserName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(name);
        }

        /// <summary>
        /// Gets the password hash for a user.
        /// </summary>
        /// <param name="user">The user to retrieve the password hash for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the password hash for the user.</returns>
        public virtual Task<string> GetPasswordHashAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string hash;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                hash = user.PasswordHash;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(hash);
        }

        /// <summary>
        /// Gets the telephone number, if any, for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
        public virtual Task<string> GetPhoneNumberAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string phone;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                phone = user.PhoneNumber;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(phone);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
        /// telephone number otherwise false.
        /// </returns>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool confirmed;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                confirmed = user.PhoneNumberConfirmed;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<bool>(ex);
            }

            return Task.FromResult(confirmed);
        }

        /// <summary>
        /// Retrieves the roles the specified <paramref name="user"/> is a member of.
        /// </summary>
        /// <param name="user">The user whose roles should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the roles the user is a member of.</returns>
        public virtual Task<IList<string>> GetRolesAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<string> roles;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                roles = user.Roles.Select(r => r.Name).ToList();
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<IList<string>>(ex);
            }

            return Task.FromResult(roles);
        }

        /// <summary>
        /// Get the security stamp for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetSecurityStampAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string stamp;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                stamp = user.SecurityStamp;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(stamp);
        }

        /// <summary>
        /// Returns the token value.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task<string> GetTokenAsync(
            TUser user,
            string loginProvider,
            string name,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var entry = await this.FindToken(user, loginProvider, name, cancellationToken);

            return entry?.Value;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified
        /// <paramref name="user"/> has two factor authentication enabled or not.
        /// </returns>
        public virtual Task<bool> GetTwoFactorEnabledAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool enabled;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                enabled = user.TwoFactorEnabled;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<bool>(ex);
            }

            return Task.FromResult(enabled);
        }

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetUserIdAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string userId;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                userId = this.ConvertIdToString(user.Id);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(userId);
        }

        /// <summary>
        /// Gets the user name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetUserNameAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string name;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                name = user.UserName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<string>(ex);
            }

            return Task.FromResult(name);
        }

        /// <summary>
        /// Retrieves all users with the specified claim.
        /// </summary>
        /// <param name="claim">The claim whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that contain the specified claim.
        /// </returns>
        public virtual Task<IList<TUser>> GetUsersForClaimAsync(
            Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<TUser> users;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (claim == null)
                {
                    throw new ArgumentNullException(nameof(claim));
                }

                var query = from userclaims in this.UserClaims
                            join user in this.Users on userclaims.User.Id equals user.Id
                            where userclaims.ClaimValue == claim.Value
                                  && userclaims.ClaimType == claim.Type
                            select user;

                users = query.ToList();
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<IList<TUser>>(ex);
            }

            return Task.FromResult(users);
        }

        /// <summary>
        /// Retrieves all users in the specified role.
        /// </summary>
        /// <param name="normalizedRoleName">The role whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that are in the specified role.
        /// </returns>
        public virtual Task<IList<TUser>> GetUsersInRoleAsync(
            string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<TUser> users;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (string.IsNullOrEmpty(normalizedRoleName))
                {
                    throw new ArgumentNullException(nameof(normalizedRoleName));
                }

                var role =
                    this.Roles.FirstOrDefault(x => x.NormalizedName == normalizedRoleName);

                users = role?.Users.ToList() ?? new List<TUser>();
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<IList<TUser>>(ex);
            }

            return Task.FromResult(users);
        }

        /// <summary>
        /// Returns a flag indicating if the specified user has a password.
        /// </summary>
        /// <param name="user">The user to retrieve the password hash for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a flag indicating if the specified user has a password. If the
        /// user has a password the returned value with be true, otherwise it will be false.</returns>
        public virtual Task<bool> HasPasswordAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool synced;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                synced = user.PasswordHash != null;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<bool>(ex);
            }

            return Task.FromResult(synced);
        }

        /// <summary>
        /// Records that a failed access has occurred, incrementing the failed access count.
        /// </summary>
        /// <param name="user">The user whose cancellation count should be incremented.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the incremented failed access count.</returns>
        public virtual Task<int> IncrementAccessFailedCountAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int count;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.AccessFailedCount++;
                count = user.AccessFailedCount;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(count);
        }

        /// <summary>
        /// Returns a flag indicating if the specified user is a member of the give <paramref name="normalizedRoleName"/>.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="normalizedRoleName">The role to check membership of</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a flag indicating if the specified user is a member of the given group. If the
        /// user is a member of the group the returned value with be true, otherwise it will be false.</returns>
        public virtual Task<bool> IsInRoleAsync(
            TUser user,
            string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool isInRole;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (string.IsNullOrWhiteSpace(normalizedRoleName))
                {
                    throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
                }

                isInRole = user.Roles.Any(r => r.NormalizedName == normalizedRoleName);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<bool>(ex);
            }

            return Task.FromResult(isInRole);
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task RemoveClaimsAsync(
            TUser user,
            IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (claims == null)
                {
                    throw new ArgumentNullException(nameof(claims));
                }

                foreach (var claim in claims)
                {
                    var matchedClaims =
                        user.Claims.Where(
                            uc =>
                            uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type)
                            .ToList();

                    foreach (var c in matchedClaims)
                    {
                        user.Claims.Remove(c);
                    }
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes the given <paramref name="normalizedRoleName"/> from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the role from.</param>
        /// <param name="normalizedRoleName">The role to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task RemoveFromRoleAsync(
            TUser user,
            string normalizedRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (string.IsNullOrWhiteSpace(normalizedRoleName))
                {
                    throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
                }

                var role = user.Roles.SingleOrDefault(r => r.NormalizedName == normalizedRoleName);

                if (role != null)
                {
                    user.Roles.Remove(role);
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the login from.</param>
        /// <param name="loginProvider">The login to remove from the user.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task RemoveLoginAsync(
            TUser user,
            string loginProvider,
            string providerKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var entry =
                    user.Logins.SingleOrDefault(
                        l =>
                        l.LoginProvider == loginProvider
                        && l.ProviderKey == providerKey);

                if (entry != null)
                {
                    user.Logins.Remove(entry);
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Deletes a token for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task RemoveTokenAsync(
            TUser user,
            string loginProvider,
            string name,
            CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var userId = user.Id;
                var entry =
                    this.UserTokens.SingleOrDefault(
                        l =>
                        l.User.Id.Equals(userId)
                        && l.LoginProvider == loginProvider
                        && l.Name == name);

                if (entry != null)
                {
                    this.Context.Delete(entry);
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The role to replace the claim on.</param>
        /// <param name="claim">The claim replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task ReplaceClaimAsync(
            TUser user,
            Claim claim,
            Claim newClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (claim == null)
                {
                    throw new ArgumentNullException(nameof(claim));
                }

                if (newClaim == null)
                {
                    throw new ArgumentNullException(nameof(newClaim));
                }

                var matchedClaims =
                    user.Claims.Where(
                        uc =>
                        uc.ClaimValue == claim.Value
                        && uc.ClaimType == claim.Type)
                        .ToList();

                foreach (var matchedClaim in matchedClaims)
                {
                    matchedClaim.ClaimValue = newClaim.Value;
                    matchedClaim.ClaimType = newClaim.Type;
                }
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Resets a user's failed access count.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>This is typically called after the account is successfully accessed.</remarks>
        public virtual Task ResetAccessFailedCountAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.AccessFailedCount = 0;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the <paramref name="email"/> address for a <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="email">The email to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailAsync(
            TUser user,
            string email,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.Email = email;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the flag indicating whether the specified <paramref name="user"/>'s email address has been confirmed or not.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailConfirmedAsync(
            TUser user,
            bool confirmed,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.EmailConfirmed = confirmed;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set the flag indicating if the specified <paramref name="user"/> can be locked out..
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be set.</param>
        /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEnabledAsync(
            TUser user,
            bool enabled,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.LockoutEnabled = enabled;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEndDateAsync(
            TUser user,
            DateTimeOffset? lockoutEnd,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.LockoutEnd = lockoutEnd;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the normalized email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetNormalizedEmailAsync(
            TUser user,
            string normalizedEmail,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.NormalizedEmail = normalizedEmail;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the given normalized name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="normalizedName">The normalized name to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedUserNameAsync(
            TUser user,
            string normalizedName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.NormalizedUserName = normalizedName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the password hash for a user.
        /// </summary>
        /// <param name="user">The user to set the password hash for.</param>
        /// <param name="passwordHash">The password hash to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPasswordHashAsync(
            TUser user,
            string passwordHash,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.PasswordHash = passwordHash;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the telephone number for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberAsync(
            TUser user,
            string phoneNumber,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.PhoneNumber = phoneNumber;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets a flag indicating if the specified <paramref name="user"/>'s phone number has been confirmed..
        /// </summary>
        /// <param name="user">The user whose telephone number confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberConfirmedAsync(
            TUser user,
            bool confirmed,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.PhoneNumberConfirmed = confirmed;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the provided security <paramref name="stamp"/> for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="stamp">The security stamp to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetSecurityStampAsync(
            TUser user,
            string stamp,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.SecurityStamp = stamp;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the token value for a particular user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The value of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task SetTokenAsync(
            TUser user,
            string loginProvider,
            string name,
            string value,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var token = await this.FindToken(user, loginProvider, name, cancellationToken);

            if (token == null)
            {
                this.Context.Save(this.CreateUserToken(user, loginProvider, name, value));
            }
            else
            {
                token.Value = value;
            }
        }

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetTwoFactorEnabledAsync(
            TUser user,
            bool enabled,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                user.TwoFactorEnabled = enabled;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the given <paramref name="userName" /> for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="userName">The user name to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetUserNameAsync(
            TUser user,
            string userName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.ThrowIfDisposed();
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }
                user.UserName = userName;
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<int>(ex);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Updates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public virtual async Task<IdentityResult> UpdateAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Context.Update(user);

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
        /// Create a new entity representing a user claim.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        protected abstract TUserClaim CreateUserClaim(TUser user, Claim claim);

        /// <summary>
        /// Create a new entity representing a user login.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        protected abstract TUserLogin CreateUserLogin(TUser user, UserLoginInfo login);

        /// <summary>
        /// Create a new entity representing a user token.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginProvider"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract TUserToken CreateUserToken(TUser user, string loginProvider, string name, string value);

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
        /// Used to attach child entities to the User aggregate, i.e. Roles, Logins, and Claims.
        /// </summary>
        /// <remarks>Uses Query method, this means NHibernate will always hit the database.</remarks>
        protected virtual Task<TUser> GetUserAggregateAsync(Expression<Func<TUser, bool>> filter)
        {
            IQueryable<TUser> query;
            try
            {
                query = this.Context.Query<TUser>().Where(filter);
                query.Fetch(p => p.Roles).ToFuture();
                query.Fetch(p => p.Claims).ToFuture();
                query.Fetch(p => p.Logins).ToFuture();
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TUser>(ex);
            }

            return Task.FromResult(query.ToFuture().FirstOrDefault());

            // In this context it's probably best not to use Task.Run but to use Task.FromResult after doing our synchronous work as above becuase...
            // http://blog.stephencleary.com/2013/11/taskrun-etiquette-examples-dont-use.html
            // http://www.ben-morris.com/why-you-shouldnt-create-asynchronous-wrappers-with-task-run/
            // return Task.Run(() =>
            // {
            // var query = Context.Query<TUser>().Where(filter);
            // query.Fetch(p => p.Roles).ToFuture();
            // query.Fetch(p => p.Claims).ToFuture();
            // query.Fetch(p => p.Logins).ToFuture();
            // return query.ToFuture().FirstOrDefault();
            // });
        }

        ///// <summary>
        ///// Eagerly loads a user, their roles, claims and logins by Id.
        ///// </summary>
        ///// <remarks>Uses Get method, this allows NHibernate to use it's cache when it can.</remarks>
        //protected Task<TUser> GetUserAggregateByIdAsync(TUserKey userId)
        //{
        //    TUser user;
        //    try
        //    {
        //        user = this.Context.Get<TUser>(userId);

        //        if (user != null)
        //        {
        //            NHibernateUtil.Initialize(user.Roles);
        //            NHibernateUtil.Initialize(user.Claims);
        //            NHibernateUtil.Initialize(user.Logins);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return AsyncHelper.FromException<TUser>(ex);
        //    }

        //    return Task.FromResult(user);

        //    // In this context it's probably best not to use Task.Run but to use Task.FromResult after doing our synchronous work as above becuase...
        //    // http://blog.stephencleary.com/2013/11/taskrun-etiquette-examples-dont-use.html
        //    // http://www.ben-morris.com/why-you-shouldnt-create-asynchronous-wrappers-with-task-run/
        //    // return Task.Run(() =>
        //    // {
        //    // var user = Context.Get<TUser>(userId);
        //    // if (user != null)
        //    // {
        //    // NHibernateUtil.Initialize(user.Roles);
        //    // NHibernateUtil.Initialize(user.Claims);
        //    // NHibernateUtil.Initialize(user.Logins);
        //    // }
        //    // return user;
        //    // });
        //}

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected Task SaveChanges(CancellationToken cancellationToken)
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

        private Task<TUserToken> FindToken(
            TUser user,
            string loginProvider,
            string name,
            CancellationToken cancellationToken)
        {
            TUserToken token;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var userId = user.Id;

                token =
                    this.UserTokens.SingleOrDefault(
                        l =>
                        l.User.Id.Equals(userId)
                        && l.LoginProvider == loginProvider
                        && l.Name == name);
            }
            catch (Exception ex)
            {
                return AsyncHelper.FromException<TUserToken>(ex);
            }

            return Task.FromResult(token);
        }
    }
}