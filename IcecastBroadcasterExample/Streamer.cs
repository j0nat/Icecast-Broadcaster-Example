using System;
using System.IO;

namespace IcecastBroadcasterExample
{
    class Streamer
    {
        private string audioDirectory = "/home/test";
        private Libshout icecast; // icecast wrapper
        
        public void Start()
        {
            // Open icecast channel
            if (OpenIcecastChannel())
            {
                Console.WriteLine("Starting stream.");

                Run();
            }
            else
            {
                Console.WriteLine("Unable to open icecast connection.");
            }
        }

        private bool OpenIcecastChannel()
        {
            icecast = new Libshout();
            icecast.setProtocol(0);
            icecast.setHost("127.0.0.1");
            icecast.setPort(8000);
            icecast.setPassword("123");
            icecast.setFormat(Libshout.FORMAT_OGG);
            icecast.setPublic(true);
            icecast.setDescription("Icecast Broadcaster Example");
            icecast.setName("Test Radio Example");
            icecast.setMount("test");
            icecast.open();

            if (icecast.isConnected())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Run()
        {
            // Start at the top of the folder - playing the first song
            int currentPlayIndex = 0;

            if (!Directory.Exists(audioDirectory))
            {
                Console.WriteLine("No audio directory found.");
            }

            string[] songs = Directory.GetFiles(audioDirectory, "*.ogg");

            // No songs found
            if (songs.Length == 0)
            {
                Console.WriteLine("No audio files found.");
                return;
            }

            while (true)
            {
                try
                {
                    // Reached end of folder - start from beginning
                    if (songs.Length <= currentPlayIndex)
                    {
                        currentPlayIndex = 0;
                    }

                    try
                    {
                        if (File.Exists(songs[currentPlayIndex]))
                        {
                            // Play song
                            Console.WriteLine("Streaming " + songs[currentPlayIndex]);

                            StreamAudioFile(songs[currentPlayIndex]);
                        }
                        else
                        {
                            Console.WriteLine("File not found. Skipping audio.");
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Error. Skipping song. " + err.ToString());
                    }

                    // Play next song
                    currentPlayIndex++;
                }
                catch
                {
                    break;
                }
            }
        }

        private void StreamAudioFile(string fileName)
        {
            byte[] buffer = new byte[4096];
            int read;

            BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open));

            int total = 0;
            while (true)
            {
                read = reader.Read(buffer, 0, buffer.Length);
                total = total + read;

                if (read > 0)
                {
                    // Use the libshout wrapper to send the audio
                    // It handles delay and makes sure the stream is smooth
                    icecast.send(buffer, read);
                }
                else
                {
                    break;
                }
            }

            reader.Close();
            reader.Dispose();
        }
    }
}
