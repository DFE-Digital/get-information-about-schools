namespace Edubase.Common.Cache
{
    public enum eCacheEvent
    {
        None,
        ConnectedToServer,
        KeySetCentrally,
        KeySetInMemory,
        KeyDeletedCentrally,
        KeyDeletedInMemory,
        KeyValueGotFromCentral,
        KeyValueGotFromMemory, 
        KeyValueGotFromCentralAttempt,
        KeyValueGotFromMemoryAttempt,
        Exception
    }
}
