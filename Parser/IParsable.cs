namespace Parser
{
    /// <summary>
    /// Parser contract which get type <see cref="T"/> and return <see cref="IParsable"/> type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParser<T>
    {
        IParsable Parse(T data);
    }

    public interface IParsable : ISaveable
    {
        string FilePath { get; set; }
        IParsable Clone();
    }
}