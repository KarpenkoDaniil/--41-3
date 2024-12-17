using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoader
{
    public static class TimeDeltaHelper
    {
        private static bool _isStart = false;
        public static bool IsStart => _isStart;
        private static Stopwatch _stopwatch = new Stopwatch();
        private static int _framesCounter = 0;
        private static int _fps = 0;
        public static int FPS { get => _fps; }
        private static long _previousFPSMeasurementTime;
        private static long _previousTicks;

        public static float TimeSpeed = 1f;

        private static float _time;
        public static float Time { get => _time; }

        private static float _deltaT;
        public static float DeltaT { get => _deltaT; }

        public static void Start()
        {
            _stopwatch.Reset();
            _framesCounter = 0;
            _fps = 0;
            _stopwatch.Start();
            _previousFPSMeasurementTime = _stopwatch.ElapsedMilliseconds;
            _previousTicks = _stopwatch.Elapsed.Ticks;
            _isStart = true;
        }

        public static void Update()
        {
            long ticks = _stopwatch.Elapsed.Ticks;
            _time = (float)ticks / TimeSpan.TicksPerSecond;
            _deltaT = (float)(ticks - _previousTicks) / TimeSpan.TicksPerSecond;
            _deltaT = _deltaT * TimeSpeed;
            _previousTicks = ticks;

            _framesCounter++;
            if (_stopwatch.ElapsedMilliseconds - _previousFPSMeasurementTime >= 1000)
            {
                _fps = _framesCounter;
                _framesCounter = 0;
                _previousFPSMeasurementTime = _stopwatch.ElapsedMilliseconds;
            }
        }
    }
}
