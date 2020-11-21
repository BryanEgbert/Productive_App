using System;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Components
{
    public class Timer : ComponentBase
    {
        public int Hours { get; set; } = 13;
        public int Minutes { get; set; } = 1;
        public int Seconds { get; set; } = 1;
        private System.Timers.Timer aTimer;
        public void StartTimer() 
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += CountDownTimer;
            if(aTimer.Enabled == true)
            {
                aTimer.Enabled = false;
            } else
            {
                aTimer.Enabled = true;
            }
        }
        public void CountDownTimer(Object source, ElapsedEventArgs e)
        {
            if(Seconds == 0)
            {
                Minutes -= 1;
                Seconds = 59;
            } else if(Minutes == 0)
            {
                Hours -= 1;
                Minutes = 59;
                Seconds = 59;
            } else if(Hours == 0 && Minutes == 0 && Seconds == 0)
            {
                aTimer.Enabled = false;
            } else
            {
                Seconds -= 1;
            }
            InvokeAsync(StateHasChanged);
        }
    }
}