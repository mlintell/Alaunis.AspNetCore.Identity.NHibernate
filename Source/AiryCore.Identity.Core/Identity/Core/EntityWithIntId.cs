namespace AiryCore.Identity.Core
{
    /// <summary>
    ///     Base class for entities that have a unique Id that is an <c>int</c>.
    /// </summary>
    /// <remarks>
    ///     This is just a shortcut for <see cref="EntityWithId{Int32}"/>.
    ///     If you want an entity with a identity of a type other than <c>int</c>,
    ///     such as string, then use <see cref="EntityWithId{TId}" /> instead.
    /// </remarks>
    public abstract class EntityWithIntId : EntityWithId<int>
    {
    }
}