using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Store;
using Lucene.Net.Linq;
using Edubase.Data.Entity;
using MoreLinq;
using System;
using FileSystemDirectory = System.IO.Directory;
using Edubase.Common.IO;
using Ionic.Zip;
using LuceneVer = Lucene.Net.Util.Version;
using Edubase.Common;

namespace Edubase.Services.Lucene
{
    /// <summary>
    /// Responsible for indexing Establishments inside a Lucene index
    /// and exposing the index for querying.
    /// </summary>
    public class EstablishmentsIndex : IDisposable
    {
        private const string BLOB_CONTAINER_NAME = "edubase-estab-index";
        private const string BLOB_FILENAME = "lucene-index.zip";

        private MMapDirectory _luceneDirectory = null;
        private LuceneDataProvider _luceneDataProvider = null;
        private DirectoryInfo _luceneDirectoryPath = null;
        private bool _isInitialised = false;

        private static Lazy<EstablishmentsIndex> _instance = new Lazy<EstablishmentsIndex>(() => new EstablishmentsIndex());
        public static EstablishmentsIndex Instance => _instance.Value;
        
        public IQueryable<Establishment> Establishments
        {
            get
            {
                if (!_isInitialised) throw new Exception("Index not initialised");
                return _luceneDataProvider.AsQueryable(EstablishmentIndexConfig.CreateMapping());
            }
        }

        public async Task InitialiseAsync(TextWriter logger)
        {
            if (logger != null) await logger?.WriteLineAsync("About to download index file");

            var svc = new BlobService();
            var blob = svc.GetBlobReference(BLOB_CONTAINER_NAME, BLOB_FILENAME);
            var zipFileLocation = FileHelper.GetTempFileName("zip");
            await blob.DownloadToFileAsync(zipFileLocation, FileMode.Create);

            if (logger != null) await logger.WriteLineAsync("Downloaded file");

            var luceneDirectoryPath = DirectoryHelper.CreateTempDirectory();
            using (var zip = new ZipFile(zipFileLocation))
                zip.ExtractAll(luceneDirectoryPath.FullName);
            File.Delete(zipFileLocation);

            if (logger != null) await logger.WriteLineAsync("Extracted zip file");

            var luceneDirectory = new MMapDirectory(luceneDirectoryPath);
            var luceneDataProvider = new LuceneDataProvider(luceneDirectory, LuceneVer.LUCENE_30);
            luceneDataProvider.Settings.EnableMultipleEntities = false;
            
            if (logger != null) await logger.WriteLineAsync("Opened index successfully");

            using (Disposer.Capture(_luceneDataProvider, _luceneDirectory))
            {
                _luceneDirectoryPath = luceneDirectoryPath;
                _luceneDirectory = luceneDirectory;
                _luceneDataProvider = luceneDataProvider;
            }

            _isInitialised = true;
        }

        public async Task<LogTextWriter> InitialiseAsync()
        {
            var log = new LogTextWriter();
            await InitialiseAsync(log);
            return log;
        }

        public async Task RebuildEstablishmentsIndexAsync(TextWriter log)
        {
            var indexLocation = DirectoryHelper.CreateTempDirectory();
            log.WriteLine($"Created new index location at {indexLocation.FullName}");

            using (var directory = new MMapDirectory(indexLocation))
            {
                using (var provider = new LuceneDataProvider(directory, LuceneVer.LUCENE_30))
                {
                    provider.Settings.EnableMultipleEntities = false;

                    using (var dc = new ApplicationDbContext())
                    {
                        using (var session = provider.OpenSession(EstablishmentIndexConfig.CreateMapping()))
                        {
                            log.WriteLine($"Starting to index {dc.Establishments.Count()} establishment record(s).");
                            dc.Establishments.Batch(100).ForEach(batch => session.Add(batch.ToArray()));
                            log.WriteLine("...done");
                        }
                    }
                }
            }

            log.WriteLine();
            log.WriteLine("Creating Lucene index zip file...");
            var indexZipFileLocation = FileHelper.GetTempFileName("zip");
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(indexLocation.FullName);
                zip.Save(indexZipFileLocation);
            }
            log.WriteLine("...done");
            log.WriteLine();

            log.WriteLine("Uploading index to blob storage");
            var fileHelper = new FileHelper();
            var blobService = new BlobService().Initialise("edubase-estab-index", false, true);
            await blobService.UploadAsync(indexZipFileLocation, fileHelper.GetMimeType(indexZipFileLocation), "edubase-estab-index", "lucene-index.zip");
            log.WriteLine("...done");
            log.WriteLine();

            log.WriteLine("Cleaning up...");
            FileSystemDirectory.Delete(indexLocation.FullName, true);
            File.Delete(indexZipFileLocation);
            log.WriteLine("...done");
        }


        public async Task<LogTextWriter> RebuildEstablishmentsIndexAsync()
        {
            var log = new LogTextWriter();
            await RebuildEstablishmentsIndexAsync(log);
            return log;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_isInitialised)
                    {
                        _luceneDataProvider.Dispose();
                        _luceneDataProvider = null;
                        _luceneDirectory.Dispose();
                        _luceneDirectory = null;
                        _luceneDirectoryPath = null;
                    }
                }
                disposedValue = true;
            }
        }
        
        public void Dispose() => Dispose(true);
        #endregion
    }
}
