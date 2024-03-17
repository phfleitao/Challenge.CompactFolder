namespace CompactFolder.Domain.Operations.Contracts
{
    public interface IExclusionRule
    {
        bool IsExcluded(string item);
    }
}
