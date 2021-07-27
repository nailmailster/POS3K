using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Threading;
using System.Windows.Media.Animation;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        UpdateProgressBarDelegate updateProgressBarDelegate;
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        Window1 window1;
        Window2 window2;

        public ProgressBarWindow()
        {
            InitializeComponent();
            updateProgressBarDelegate = new UpdateProgressBarDelegate(progressBarEquipment.SetValue);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Vars.equipmentIsOk)
            {
                //  вместо того чтобы всякий раз создавать окно меню
                //window1 = new Window1();
                //window1.Show();
                //  в Window1.xaml.cs мы привязали окно меню владельцем этого окна (окна прогресса) и теперь можем к нему обращаться
                Owner.Show();
                //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
                DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //doubleAnimation.Completed += DoubleAnimationOpacity_Completed;
                //  стартуем анимацию непрозрачности
                Owner.BeginAnimation(OpacityProperty, doubleAnimation);
            }
            else
            {
                window2 = new Window2();
                window2.Owner = this.Owner;
                window2.Show();
            }
        }

        //private void DoubleAnimationOpacity_Completed(object sender, EventArgs e)
        //{
        //}

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            for (double i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 0, 0, 1));
                Dispatcher.Invoke(updateProgressBarDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, i });
                //Dispatcher.Invoke(new Action((p, v) => progressBarEquipment.Value = v), progressBarEquipment, i);
            }
            progressBarEquipment.Value = 0;
            Vars.equipmentIsOk = true;

            Close();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }
    }
}
