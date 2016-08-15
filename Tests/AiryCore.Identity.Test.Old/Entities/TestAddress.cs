namespace AiryCore.Identity.Test.Entities
{
    using System;

    using AiryCore.Identity.Core;

    /// <summary>
    /// Test Address class for testing One To Many mapping conventions.
    /// </summary>
    public class TestAddress : EntityWithId<Guid>
    {
        public virtual string Line1 { get; set; }
    }
}