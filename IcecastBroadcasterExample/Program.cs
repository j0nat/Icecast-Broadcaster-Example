using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcecastBroadcasterExample
{
    class Program
    {
        private static Streamer streamer;

        static void Main(string[] args)
        {
            streamer = new Streamer();

            streamer.Start();
        }
    }
}
