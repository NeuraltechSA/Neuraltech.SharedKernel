public interface IPaginationConstraints
{
    static abstract long MaxPageLimit { get; }
    static abstract long MaxSizeLimit { get; }
}
