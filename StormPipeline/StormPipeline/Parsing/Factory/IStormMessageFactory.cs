namespace Com.Apdcomms.StormPipeline.Parsing.Factory
{
    public interface IStormMessageFactory<T>
    {
        T Create(string messageCode);
    }
}
