namespace Arb_Int.Domain
{
    public enum SystemHistoryPolicy
    {
        Full,
        PseudonymizeOnErasure,
        Excluded
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class SystemHistoryPolicyAttribute(SystemHistoryPolicy policy) : Attribute
    {
        public SystemHistoryPolicy Policy { get; } = policy;
    }
}
