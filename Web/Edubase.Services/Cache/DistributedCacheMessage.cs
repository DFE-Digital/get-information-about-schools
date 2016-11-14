using System;

namespace Edubase.Services.Cache
{
    /// <summary>
    /// Represents a cache message used by the distributed cache architecture.
    /// For instance, instructs key updates/deletes on all nodes.
    /// </summary>
    [Serializable]
    internal class DistributedCacheMessage
    {
        public const string MESSAGE_ALREADY_PROCESSED_SPECIFIER = "1";
        public string Key { get; set; }
        public byte[] Value { get; set; }
        public Guid Id { get; private set; }
        public string TransactionCacheKey { get { return string.Concat("DistributedCacheMessage_", Id); } }
        public string SenderCacheName { get; set; }

        public DistributedCacheMessage()
        {
            Id = Guid.NewGuid();
        }
    }
}
