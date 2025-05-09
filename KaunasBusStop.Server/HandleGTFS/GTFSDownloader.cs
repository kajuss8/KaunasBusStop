using System.IO.Compression;

namespace KaunasBusStop.Server.HandleGTFS
{
    public class GTFSDownloader
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task DownloadAndUnzipGTFSFileAsync(string url, string destinationFolder)
        {
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var zipStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            var filePath = Path.Combine(destinationFolder, entry.FullName);
                            var fileDirectory = Path.GetDirectoryName(filePath);

                            if (!Directory.Exists(fileDirectory))
                            {
                                Directory.CreateDirectory(fileDirectory);
                            }

                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await entryStream.CopyToAsync(fileStream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DownloadAndUnzipGTFSFileAsync failed", ex);
            }
        }
    }
}
