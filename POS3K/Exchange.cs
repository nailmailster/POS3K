using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Timers;
using System.IO;
using System.Windows;
using System.Windows.Threading;

using System.Data;
using System.Data.OleDb;

namespace POS3K
{
    public class Exchange
    {
        public Timer timer;
        //string s;

        string FilePath = Properties.Settings.Default.DBF;
        public OleDbConnection exchangeConnection;

        public Exchange()
        {
            exchangeConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");

            timer = new Timer(5000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //timer.Enabled = false;
            if (Directory.Exists(Properties.Settings.Default.InPath))
                Vars.autonomode = false;
            else
            {
                Vars.autonomode = true;
                Vars.exchangeIsActive = false;
                return;
            }
            if (Vars.exchangeIsActive)
            {
                Vars.autonomode = false;
                return;
            }
            if (!Vars.goodsIsBusy)
            {
                Vars.exchangeIsActive = true;
                if (exchangeConnection.State != ConnectionState.Open)
                    exchangeConnection.Open();

                if (exchangeConnection.State == ConnectionState.Open)
                {
                    if (!GoodsDBF.CheckAndUpdateOrAddWholeDirectory(exchangeConnection))
                    {
                        Vars.exchangeIsActive = false;
                        timer.Enabled = true;
                        return;
                    }
                }
                if (!LoyaltyDBF.UpdateFromLoys(Properties.Settings.Default.InPath))
                {
                    Vars.exchangeIsActive = false;
                    timer.Enabled = true;
                    return;
                }
                if (!LoyaltyDBF.UpdateFromLoys(Properties.Settings.Default.LoyaltyPath + @"\In"))
                {
                    Vars.exchangeIsActive = false;
                    timer.Enabled = true;
                    return;
                }
                if (!LoyaltyDBF.UpdateFromLyts(Properties.Settings.Default.InPath))
                {
                    Vars.exchangeIsActive = false;
                    timer.Enabled = true;
                    return;
                }
                if (!LoyaltyDBF.UpdateFromLyts(Properties.Settings.Default.LoyaltyPath + @"\In"))
                {
                    Vars.exchangeIsActive = false;
                    timer.Enabled = true;
                    return;
                }
            }
            Vars.exchangeIsActive = false;
            //timer.Enabled = true;
        }
    }
}
