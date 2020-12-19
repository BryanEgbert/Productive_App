using System;
using System.Timers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Components
{
    public class TimerStructure
    {
        public  int Id { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }      
    }
    public class Timer : ComponentBase
    {
        public List<TimerStructure> timerCollections = new List<TimerStructure>()
        {
            new TimerStructure(){ Id = 1, Hours = 0, Minutes = 30, Seconds = 0 },
            new TimerStructure(){ Id = 2, Hours = 1, Minutes = 0, Seconds = 0 },
            new TimerStructure(){ Id = 3, Hours = 1, Minutes = 30, Seconds = 0 },
        };

        public int Index { get; private set; }
        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public int Seconds { get; set; } = 0;
        public bool StopButtonIsDisabled { get; set; } = true;
        public bool StartButtonIsDisabled { get; set; } = false;
        public bool SetTimerButtonIsDisabled { get; set; } = false;
        private static System.Timers.Timer aTimer;
        public void SetTimer(int value)
        {
            this.Index = value - 1;
            Hours = timerCollections[Index].Hours;
            Minutes = timerCollections[Index].Minutes;
            Seconds = timerCollections[Index].Seconds;
        }
        public void StopTimer()
        {
            aTimer.Stop();
            aTimer.Dispose();

            StopButtonIsDisabled = true;
            StartButtonIsDisabled = false;
            SetTimerButtonIsDisabled = false;

            Console.WriteLine($"{Hours}:{Minutes}:{Seconds}");
        }
        public void StartTimer() 
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += CountDownTimer;
            aTimer.Start(); 

            StopButtonIsDisabled = false;   
            StartButtonIsDisabled = true;
            SetTimerButtonIsDisabled = true;
        }
        public void CountDownTimer(Object source, ElapsedEventArgs e)
        {
            if(Seconds == 0 && Minutes > 0)
            {
                Minutes -= 1;
                Seconds = 59;
            } else if (Minutes == 0 && Seconds == 0 && Hours > 0)
            {
                Hours -= 1;
                Minutes = 59;
                Seconds = 59;
            } else if (Hours == 0 && Minutes == 0 && Seconds == 0)
            {
                aTimer.Stop();
                aTimer.Dispose();

                StopButtonIsDisabled = true;
                StartButtonIsDisabled = false;
                SetTimerButtonIsDisabled = false;
            } else
            {
                Seconds -= 1;
            }
            InvokeAsync(StateHasChanged);
        }
    }
}