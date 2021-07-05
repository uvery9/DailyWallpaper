using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DailyWallpaper.Helpers
{
    class TimerHelper: IDisposable
    {  
        private static TimerHelper _instance;

        /// <summary>
        /// single instance,
        /// 1000 * 60 * 10  = 10 mins, 
        /// use minutes rather than hours, easy to calculate the rest time
        /// </summary>
        /// <param name="mins"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static TimerHelper GetInstance(int mins, ElapsedEventHandler handler)
        {
            return _instance ?? (_instance = new TimerHelper(mins, handler));
        }

        private Timer _timer;
        private TimerHelper(int mins, ElapsedEventHandler handler)
        {
            _timer = new Timer
            {
                Interval = 1000 * 60 * mins,
                AutoReset = true,
                Enabled = true
            };
            // _timer.
            _timer.Elapsed += handler;
            _timer.Start();
        }

        /// <summary>
        /// use minutes rather than hours.
        /// </summary>
        /// <param name="mins"></param>
        public void SetTimer(int mins)
        {
            _timer.Interval = 1000 * 60 *  mins;
        }
        public void SetTimer(int mins, Action<int> doAfter)
        {
            _timer.Interval = 1000 * 60 * mins;
            doAfter(mins);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        /// <summary>
        /// Second usage
        /// </summary>
        public delegate void ScheduleCallBackFunc();
        private int _hour;
        private int _minute;
        private ScheduleCallBackFunc _callback;

        /// <summary>
        /// ScheduleCallBackFunc
        /// </summary>
        /// <param name="iHour"></param>
        /// <param name="iMinute"></param>
        /// <param name="callback"></param>
        public TimerHelper(int iHour, int iMinute, ScheduleCallBackFunc callback)
        {
            _hour = iHour;
            _minute = iMinute;
            _callback = callback;
            var aTimer = new Timer
            {
                // 1min check one time.
                Interval = 60 * 1000,
                Enabled = true
            };
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);
        }
        private void aTimer_Elapsed(Object source, ElapsedEventArgs e)
        {
            // get hour minute second
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;

            if (intHour == _hour && intMinute == _minute)
            {
                // Do your thing.
                _callback();
            }
        }

        /// <summary>
        /// Threading Timer very useless.
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="min"></param>
        /// 
        public TimerHelper(int min, System.Threading.TimerCallback callback)
        {
            // begin, period
            new System.Threading.Timer(callback, null, 1000 * 60 * min, 1000 * 60 * min);
        }
    }
}
