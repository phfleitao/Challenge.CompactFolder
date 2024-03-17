namespace CompactFolder.Cli.Operations.Contracts
{
    public interface IOutputTypeHandlerFactory
    {
        IOutputTypeHandler Create(string outputType);
    }
}
