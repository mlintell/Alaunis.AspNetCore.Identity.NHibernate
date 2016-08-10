namespace AiryCore.Extensions.NHibernate
{
    using System;
    using System.Reflection;

    using global::NHibernate.Mapping.ByCode;

    /// <summary>
    /// Extension methods for <see cref="PropertyPath"/>.
    /// </summary>
    public static class PropertyPathExtensions
    {
        /// <summary>
        /// Gets the type of the root member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static Type GetRootMemberType(this PropertyPath member)
        {
            return member.GetRootMember().DeclaringType;
        }

        /// <summary>
        /// Gets the type of the collection element.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static Type GetCollectionElementType(this PropertyPath member)
        {
            return member.LocalMember.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
        }

        /// <summary>
        /// Called when [to many other side property].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static MemberInfo OneToManyOtherSideProperty(this PropertyPath member)
        {
            return member.GetCollectionElementType().GetFirstPropertyOfType(member.GetRootMemberType());
        }
    }
}