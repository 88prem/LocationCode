namespace Com.Apdcomms.StormPipeline.Parsing.Factory
{
    public interface IStormMesageCodeFactory<T>
    {
        T CreateCode(string messageCode, string namespaceName);
    }
}
