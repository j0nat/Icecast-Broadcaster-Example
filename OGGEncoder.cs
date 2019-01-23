using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace OGGEncoder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Requires ffmpeg.exe
            // Download from https://www.ffmpeg.org/ and place the ffmpeg.exe file in the root directory
            // Drag and drop media file to convert

            string workingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fileName = Path.GetFileNameWithoutExtension(args[0]);
            string filePath = args[1];
            string extension = ".ogg";
            string bitrate = "48k";

            if (!Directory.Exists(Path.Combine(workingDirectory, filePath)))
            {
                Directory.CreateDirectory(Path.Combine(workingDirectory, filePath));
            }

            if (File.Exists(fileName + extension))
            {
                fileName += "NEW";
            }

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = Path.Combine(workingDirectory, "ffmpeg");
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.Arguments = String.Format("-i \"{0}\" -vn -c:a libvorbis -b:a {1} -map_metadata -1 \"{2}\"", args[0], bitrate, Path.Combine(filePath, fileName + extension));
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            RemoveTags(args[0], Path.Combine(workingDirectory, filePath, fileName + extension));

            File.Delete(args[0]);
        }

        private static void RemoveTags(string oldfile, string newfile)
        {
            string title = null;
            string[] performers = null;
            string[] artists = null;

            // Extract title and performer from old song file
            using (var file = TagLib.File.Create(oldfile))
            {
                title = file.Tag.Title;

                if (file.Tag.Performers.Length >= 1)
                {
                    performers = file.Tag.Performers;
                }

                if (file.Tag.AlbumArtists.Length >= 1)
                {
                    artists = file.Tag.AlbumArtists;
                }
            }

            // Insert title and performer in new song file
            using (var file = TagLib.File.Create(newfile))
            {
                file.RemoveTags(TagLib.TagTypes.AllTags);

                if (performers != null)
                {
                    file.Tag.Performers = performers;
                }

                if (artists != null)
                {
                    file.Tag.AlbumArtists = artists;
                }

                if (title == null)
                {
                    file.Tag.Title = Path.GetFileNameWithoutExtension(newfile);
                }
                else
                {
                    file.Tag.Title = title;
                }

                file.Save();
            }
        }
    }
}