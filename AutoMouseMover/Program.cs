using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoMouseMover
{

    //https://stackoverflow.com/questions/8050825/how-to-move-mouse-cursor-using-c
    //https://stackoverflow.com/questions/8739523/directing-mouse-events-dllimportuser32-dll-click-double-click
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        public const int SampleRateInSeconds = 5;
        public const int MoveDelayInSeconds = 45;
        public const int MovePixels = 3;

        public static int MaxSamples = MoveDelayInSeconds / SampleRateInSeconds;

        static async Task Main(string[] args)
        {
            FreeConsole();
            var toMove = MovePixels;

            while (true)
            {
                var currentPosition = await WaitNextMove();
                //await Task.Delay(TimeSpan.FromSeconds(MoveDelayInSeconds));
                //GetCursorPos(out var currentPosition);
                SetCursorPos(currentPosition.X + toMove, currentPosition.Y);

                toMove *= -1;
            }
        }

        private static async Task<POINT> WaitNextMove()
        {
            var positionList = new List<POINT>();
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(SampleRateInSeconds));
                GetCursorPos(out var currentPosition);
                if(!positionList.All(it => it.Equals(currentPosition)))
                {
                    positionList.Clear();
                }

                positionList.Add(currentPosition);

                if (positionList.Count == MaxSamples)
                    return positionList[0];
            }
        }
    }

    public struct POINT
    {
        public int X;
        public int Y;
    }
}
