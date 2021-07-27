using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace POS3K
{
    public static class CouponsDBF
    {
        public static bool GetRecordBySerial(ref Coupon coupon)
        {
            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Coupons";

            coupon.CodeField = "";
            coupon.DiscountField = 0;
            coupon.CheckedField = 0;

            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from " + FileName + " where serial = '" + coupon.SerialField + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    command.CommandText = "select * from " + FileName + " where serial = '" + coupon.SerialField.ToLower() + "'";
                    dt.Load(command.ExecuteReader());
                    if (dt.Rows.Count == 0)
                    {
                        command.CommandText = "select * from " + FileName + " where serial = '" + coupon.SerialField.ToUpper() + "'";
                        dt.Load(command.ExecuteReader());
                    }
                }
                connection.Close();

                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    coupon.CodeField = dt.Rows[0].Field<string>("Code");
                    coupon.DiscountField = Convert.ToInt32(dt.Rows[0].Field<double>("Discount"));
                    //coupon.DiscountField = Convert.ToInt32(dt.Rows[0]["DISCOUNT"]);
                    coupon.CheckedField = Convert.ToInt32(dt.Rows[0]["Checked"]);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
