namespace Edubase.Services.Core
{
    /// <summary>
    /// Represents a NVP data store held on the end-user client.
    /// </summary>
    public interface IClientStorage
    {
        /// <summary>
        /// Source IP address of the client.
        /// </summary>
        string IPAddress { get; }

        string Get(string key);

        /// <summary>
        /// Saves the value on the client and returns it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string Save(string key, string value);
    }
}
