using System;
using System.Timers;

namespace FrontEnd.Pages
{
    public partial class Index
    {
        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public int Seconds { get; set; } = 0;
        public bool StopButtonIsDisabled { get; set; } = true;
        public bool StartButtonIsDisabled { get; set; } = false;
        public bool SetTimerButtonIsDisabled { get; set; } = false;
        private static System.Timers.Timer aTimer;

        // Stop timer
        public void StopTimer()
        {
            aTimer.Stop();
            aTimer.Dispose();

            StopButtonIsDisabled = true;
            StartButtonIsDisabled = false;
            SetTimerButtonIsDisabled = false;
        }
        // Start timer
        public void StartTimer() 
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += CountDownTimer;
            aTimer.Start(); 

            StopButtonIsDisabled = false;   
            StartButtonIsDisabled = true;
            SetTimerButtonIsDisabled = true;
        }
        // Countdown timer function
        public void CountDownTimer(Object source, ElapsedEventArgs e)
        {
            if(Seconds == 0 && Minutes > 0)
            {
                Minutes -= 1;
                Seconds = 59;
            } 
            else if (Minutes == 0 && Seconds == 0 && Hours > 0)
            {
                Hours -= 1;
                Minutes = 59;
                Seconds = 59;
            } 
            else if (Hours == 0 && Minutes == 0 && Seconds == 0)
            {
                aTimer.Stop();
                aTimer.Dispose();

                Hours = hours;
                Minutes = minutes;
                Seconds = seconds;

                StopButtonIsDisabled = true;
                StartButtonIsDisabled = false;
                SetTimerButtonIsDisabled = false;
            } 
            else
            {
                Seconds -= 1;
            }
            InvokeAsync(StateHasChanged);
        }
    }
}