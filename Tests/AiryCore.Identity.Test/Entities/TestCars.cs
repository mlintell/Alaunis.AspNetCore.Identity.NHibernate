namespace AiryCore.Identity.Test.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using AiryCore.Identity.Core;

    /// <summary>
    /// Test Car class for testing Many To Many mapping conventions.
    /// </summary>
    public class TestCar : EntityWithId<Guid>
    {
        [StringLength(8)]
        public virtual string RegistrationNumber { get; set; }
    }
}