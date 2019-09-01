using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using produce.Models;

namespace produce
{
    class Program
    {
        private static string ShowNumber;
        private static string RawFilePathAndName;
        private static string workingDirectory = "c:\\dev\\shownotes\\working\\";
        private static string feedDirectory = "c:\\dev\\shownotes\\feed\\";

        static void Main(string[] args)
        {
            ShowNumber = args[0];
            RawFilePathAndName = args[1];
            DoWork().GetAwaiter().GetResult();
        }

        static private async Task DoWork()
        {
            DocumentDBRepository<produce.Models.Feed>.Initialize();
            DocumentDBRepository<produce.Models.Item>.Initialize();

            //process video to audo, upload to blob, get file properties (length, size)
            ProcessAndUploadVideo();

            //write this new record to DocDB
            // await WriteToDocDB();

            //use DocDB to write the RSS Feed
            await GetRssFeed();
        }

        static private async Task GetRssFeed()
        {
            string rss = await Feed.EmitRssAsync();
            var file = File.CreateText(Path.Combine(feedDirectory, "feed.xml"));
            file.Write(rss);
            file.Close();

        }

        // static private async Task WriteToDocDB()
        // {
        //     Console.WriteLine("Hello World");
        // }

        static private void ProcessAndUploadVideo()
        {
            try
            {


                ShowNumber = String.Concat((ShowNumber.Length < 3 ? "0" : ""), ShowNumber);

                string[] fileList = Directory.GetFiles(workingDirectory);

                // Empty Working Directory
                foreach (string f in fileList)
                {
                    File.Delete(f);
                }

                File.Copy(RawFilePathAndName, Path.Combine(workingDirectory, Path.GetFileName(RawFilePathAndName)));
                string newFileNameAndPath = workingDirectory + "AFN-" + ShowNumber + ".m4a";
                using (var process = new Process())
                {
                    string tmp = String.Format("-i \"" + workingDirectory + Path.GetFileName(RawFilePathAndName) + "\" -vn -c:a copy \"" + newFileNameAndPath + "\"");

                    process.StartInfo.FileName = "c:\\dev\\shownotes\\ffmpeg\\ffmpeg.exe";
                    process.StartInfo.Arguments = tmp;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();
                }

                string videoLength = string.Empty;
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "c:\\dev\\shownotes\\ffmpeg\\ffprobe.exe";
                    string tmp = String.Format("-i \"" + newFileNameAndPath + "\" -v error -show_entries stream=duration -of default=noprint_wrappers=1:nokey=1");
                    //-v error -show_entries stream=duration -i d:\home\site\wwwroot\stripvideo\aff2017-11-10.mp3 -of default=noprint_wrappers=1:nokey=1        
                    process.StartInfo.Arguments = tmp;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    int seconds = int.Parse(output.Split('.')[0]);
                    int minutes = seconds / 60;
                    int secondsremainder = seconds - (minutes * 60);
                    string pad = string.Empty;
                    if (secondsremainder < 10)
                    {
                        pad = "0";
                    }
                    videoLength = minutes.ToString() + ":" + pad + secondsremainder.ToString();
                    var file = File.CreateText(Path.Combine(Path.GetDirectoryName(newFileNameAndPath), "duration.txt"));
                    file.Write(videoLength);
                    file.Close();
                }

                string fileSize = String.Empty;
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "c:\\dev\\shownotes\\ffmpeg\\ffprobe.exe";
                    string tmp = String.Format("-i \"" + newFileNameAndPath + "\" -v error -show_entries format=size -of default=noprint_wrappers=1:nokey=1");
                    //-v error -show_entries stream=duration -i d:\home\site\wwwroot\stripvideo\aff2017-11-10.mp3 -of default=noprint_wrappers=1:nokey=1        

                    process.StartInfo.Arguments = tmp;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    fileSize = output.Replace("\r", string.Empty).Replace("\n", string.Empty);
                    var file = File.CreateText(Path.Combine(Path.GetDirectoryName(newFileNameAndPath), "enclosureLength.txt"));
                    file.Write(fileSize);
                    file.Close();
                }

                string storageConnectionString = "";

                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
                CloudBlobClient serviceClient = account.CreateCloudBlobClient();

                // Create container. Name must be lower case.
                Console.WriteLine("Creating container...");
                var container = serviceClient.GetContainerReference("episodes");
                container.CreateIfNotExistsAsync().Wait();

                var fileBytes = File.ReadAllBytes(newFileNameAndPath);

                // write a blob to the container
                CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(newFileNameAndPath));
                blob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length).Wait();
                // Print selected subscription
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
