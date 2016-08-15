// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable UnusedTypeParameter

namespace AiryCore.Identity.Test.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AiryCore.Identity.Core.Attribute;
    using AiryCore.Identity.Entity;

    /// <summary>
    /// User class for testing AiryCore NHibernate ASP.Net Identity functionality.
    /// </summary>
    public class TestUser : IdentityUser<string, TestUserClaim, TestLogin, TestRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestUser"/> class.
        /// </summary>
        public TestUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.SetRequiredUniqueValues();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestUser"/> class with the given username.
        /// </summary>
        /// <param name="userName">A username for the user.</param>
        public TestUser(string userName)
            : this()
        {
            this.UserName = userName;
            this.SetRequiredUniqueValues();
        }

        private void SetRequiredUniqueValues()
        {
            this.TestIndex2 = Guid.NewGuid().ToString("B");
            this.TestIndex5 = Guid.NewGuid().ToString("B");
        }

        public virtual string Hometown { get; set; }

        [StringLength(38)]
        [Index("IX1")]
        public virtual string TestIndex1 { get; set; }

        [StringLength(38)]
        [Index("IU1", Unique = true)]
        public virtual string TestIndex2 { get; set; }

        [StringLength(38)]
        [Index("IX2")]
        public virtual string TestIndex3 { get; set; }

        [StringLength(38)]
        [Index("IX2")]
        [Index("IU2", Unique = true)]
        public virtual string TestIndex4 { get; set; }

        [StringLength(38)]
        [Index("IU2", Unique = true)]
        public virtual string TestIndex5 { get; set; }

        public virtual ICollection<TestAddress> Addresses { get; set; }

        public virtual ICollection<TestCar> CarsAvailable { get; set; }
    }
}