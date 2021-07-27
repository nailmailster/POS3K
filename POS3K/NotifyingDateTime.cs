using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace POS3K
{
    class NotifyingDateTime : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime now;
        public DateTime Now
        {
            get { return now; }
            set
            {
                now = value;
                if (now != value)
                    NotifyPropertyChanged("Now");
            }
        }

        public NotifyingDateTime()
        {
            now = DateTime.Now;

            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(1);
            //timer.Tick += Timer_Tick;
            //timer.Start();
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    now = DateTime.Now;
        //}
        
        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Now"));
        }
    }
}
