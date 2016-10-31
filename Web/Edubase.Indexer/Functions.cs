using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Lucene.Net.Store;
using Lucene.Net.Linq;
using Lucene.Net.Util;
using Edubase.Data.Entity;
using MoreLinq;
using System;
using FileSystemDirectory = System.IO.Directory;
using Edubase.Services;
using Edubase.Common.IO;
using Ionic.Zip;

namespace Edubase.Indexer
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public async static Task RebuildEstablishmentsIndex(TextWriter log)
        {
            var indexLocation = FileSystemDirectory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            log.WriteLine($"Created new index location at {indexLocation.FullName}");

            using (var directory = new MMapDirectory(indexLocation))
            {
                using (var provider = new LuceneDataProvider(directory, Lucene.Net.Util.Version.LUCENE_30))
                {
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
            var indexZipFileLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
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

            //using (var directory = new MMapDirectory(destination))
            //{
            //    using (var provider = new LuceneDataProvider(directory, Version.LUCENE_30))
            //    {
            //        var estabs = provider.AsQueryable(EstablishmentIndexConfig.CreateMapping());
            //        var est = from e in estabs
            //                  where e.Name.StartsWith("Abbey")
            //                  select e;

            //        int i = 0;

            //    }
            //}

        }
    }
}
