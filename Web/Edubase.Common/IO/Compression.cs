using System.IO;
using System.IO.Compression;

namespace Edubase.Common.IO
{
    public class Compression
    {
        public byte[] Compress(byte[] buffer)
        {
            if (buffer == null) return null;

            using (var uncompressedStream = new MemoryStream(buffer))
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressionStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true))    
                    {
                        uncompressedStream.CopyTo(compressionStream);
                    }
                    return compressedStream.ToArray();
                }
            }
        }

        public byte[] Decompress(byte[] buffer)
        {
            if (buffer == null) return null;

            using (var compressedStream = new MemoryStream(buffer))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    using (var decompressionStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }
                    return decompressedStream.ToArray();
                }
            }
        }
    }
}
