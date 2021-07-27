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
    public static class ChequesDBF
    {
        public static bool AddRecordsToCheqgood(ActiveCheck activeCheck)
        {
            int result = 0;

            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheqgood";

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();
            try
            {
                //cmd.CommandText = "INSERT into " + FileName + " ([Good], [Price], [Quantity], [Sum], [Dn], [Goodnum], [Rdn], [Rquantity], [Isset], [Lprices], [Lpricep], [Ldiscount], [Loyal], [Lcard], [Sapid], [Actnum]) VALUES (@Good, @Price, @Quantity, @Sum, @Dn, @Goodnum, @Rdn, @Rquantity, @Isset, @Lprices, @Lpricep, @Ldiscount, @Loyal, @Lcard, @Sapid, @Actnum)";
                //cmd.Parameters.AddWithValue("@Good", "16161616");
                //cmd.Parameters.AddWithValue("@Price", "123,45");
                //cmd.Parameters.AddWithValue("@Quantity", "1");
                //cmd.Parameters.AddWithValue("@Sum", "123,45");
                //cmd.Parameters.AddWithValue("@Dn", "201904054");
                //cmd.Parameters.AddWithValue("@Goodnum", "");
                //cmd.Parameters.AddWithValue("@Rdn", "");
                //cmd.Parameters.AddWithValue("@Rquantity", "");
                //cmd.Parameters.AddWithValue("@Isset", "");
                //cmd.Parameters.AddWithValue("@Lprices", "");
                //cmd.Parameters.AddWithValue("@Lpricep", "");
                //cmd.Parameters.AddWithValue("@Ldiscount", "");
                //cmd.Parameters.AddWithValue("@Loyal", "");
                //cmd.Parameters.AddWithValue("@Lcard", "");
                //cmd.Parameters.AddWithValue("@Sapid", "");
                //cmd.Parameters.AddWithValue("@Actnum", "");

                int recordsCount = activeCheck.specification.Count;
                foreach (ActivePosition activePosition in activeCheck.specification)
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "INSERT into " + FileName + " ([Good], [Price], [Quantity], [Sum], [Dn]) VALUES (@Good, @Price, @Quantity, @Sum, @Dn)";
                    cmd.Parameters.AddWithValue("@Good", activePosition.Barcode);
                    cmd.Parameters.AddWithValue("@Price", activePosition.Price);
                    cmd.Parameters.AddWithValue("@Quantity", activePosition.Quantity);
                    cmd.Parameters.AddWithValue("@Sum", activePosition.Sum);
                    cmd.Parameters.AddWithValue("@Dn", activeCheck.date + activeCheck.number);
                    result = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    if (result != 1)
                        return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = 0;
            }
            connection.Close();
            connection.Dispose();

            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool AddRecordToCheques(ActiveCheck activeCheck)
        {
            int result;

            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheques";

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();
            try
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT into " + FileName + " ([Date], [Time], [Number], [Cashier], [Type], [Dn], [Total], [Fiscal], [Cashiernam], [Vat7], [Vat10], [VAT15], [Vat20]) VALUES (@Date, @Time, @Number, @Cashier, @Type, @Dn, @Total, @Fiscal, @Cashiernam, @Vat7, @Vat10, @Vat15, @Vat20)";
                cmd.Parameters.AddWithValue("@Date", activeCheck.date);
                cmd.Parameters.AddWithValue("@Time", activeCheck.time);
                cmd.Parameters.AddWithValue("@Number", activeCheck.number);
                cmd.Parameters.AddWithValue("@Cashier", Vars.cashierId);
                cmd.Parameters.AddWithValue("@Type", activeCheck.type);
                cmd.Parameters.AddWithValue("@Dn", activeCheck.date + activeCheck.number);
                cmd.Parameters.AddWithValue("@Total", activeCheck.TotalSum - activeCheck.DiscountSum);
                cmd.Parameters.AddWithValue("@Fiscal", "0");
                if (Vars.cashierName.Length > 15)
                    Vars.cashierName = Vars.cashierName.Substring(0, 15);
                cmd.Parameters.AddWithValue("@Cashiernam", Vars.cashierName);
                cmd.Parameters.AddWithValue("@Vat7", activeCheck.VAT7);
                cmd.Parameters.AddWithValue("@Vat10", activeCheck.VAT10);
                cmd.Parameters.AddWithValue("@Vat15", activeCheck.VAT15);
                cmd.Parameters.AddWithValue("@Vat20", activeCheck.VAT20);
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = 0;
            }
            connection.Close();
            connection.Dispose();

            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool AddRecordToLCheques(ActiveCheck activeCheck)
        {
            int result;

            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "LCheques";

            //activeCheck.loyalty.SumSub = activeCheck.CardSum;
            //if (activeCheck.CardSum == 0)
            //    activeCheck.loyalty.SumAdd = activeCheck.TopaySum / 100 * activeCheck.loyalty.Percent;
            //activeCheck.loyalty.SumNew = activeCheck.loyalty.Sums - activeCheck.loyalty.SumSub + activeCheck.loyalty.SumAdd;

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();
            try
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT into " + FileName + " ([Dn], [Lcard], [Sumold], [Sumsub], [Sumadd], [Sumnew], [Total], [Cash], [Lcash]) VALUES (@Dn, @Lcard, @Sumold, @Sumsub, @Sumadd, @Sumnew, @Total, @Cash, @Lcash)";
                cmd.Parameters.AddWithValue("@Dn", activeCheck.date + activeCheck.number);
                cmd.Parameters.AddWithValue("@Lcard", activeCheck.loyalty.Code);
                cmd.Parameters.AddWithValue("@Sumold", activeCheck.loyalty.Sums);
                cmd.Parameters.AddWithValue("@Sumsub", activeCheck.loyalty.SumSub);
                cmd.Parameters.AddWithValue("@Sumadd", activeCheck.loyalty.SumAdd);
                cmd.Parameters.AddWithValue("@Sumnew", activeCheck.loyalty.SumNew);
                cmd.Parameters.AddWithValue("@Total", activeCheck.TotalSum - activeCheck.DiscountSum);
                cmd.Parameters.AddWithValue("@Cash", activeCheck.CashSum);
                cmd.Parameters.AddWithValue("@Lcash", activeCheck.CardSum);
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                connection.Close();
                connection.Dispose();
                MessageBox.Show(ex.Message);
                result = 0;
            }
            connection.Close();

            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool AddCheck(ActiveCheck activeCheck)
        {
            if (activeCheck.loyalty.Code == null)
            {
                if (AddRecordToCheques(activeCheck))
                    return AddRecordsToCheqgood(activeCheck);
                else
                    return false;
            }
            else
            {
                if (AddRecordToCheques(activeCheck))
                    if (AddRecordsToCheqgood(activeCheck))
                        if (AddRecordToLCheques(activeCheck))
                            return LoyaltyDBF.UpdateSums(ref activeCheck.loyalty);
                        else
                            return false;
            }
            return false;
        }

        public static string GetLatestDn(string date)
        {
            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheques";

            DataTable dt = new DataTable();

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();
            try
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT LAST (Dn) FROM " + FileName;

                dt.Load(cmd.ExecuteReader());
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "-1";
            }
            connection.Close();
            connection.Dispose();

            if (dt.Rows.Count > 0)
                return dt.Rows[0].Field<string>(0);
            else
                return "000000000000";
        }

        public static DataTable GetDataTableCheques()
        {
            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheques";

            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from " + FileName, connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                command.Dispose();

                connection.Close();
                connection.Dispose();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Пустой запрос");
                }

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static DataTable GetDataTableCheqGood(string dn)
        {
            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheqgood";

            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from " + FileName + " where dn = '" + dn + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                connection.Close();
                connection.Dispose();
                command.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static void GetTotalAndCheckSumAndVAT(string date, string time, out double total, out int checkSum, out double VAT7, out double VAT10, out double VAT15, out double VAT20)
        {
            total = 0;
            checkSum = 0;
            VAT7 = 0;
            VAT10 = 0;
            VAT15 = 0;
            VAT20 = 0;

            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheques";

            //string dateTime = DateTime.Now.AddDays(-10).ToString();
            //string dateString = dateTime.Substring(6, 4) + dateTime.Substring(3, 2) + dateTime.Substring(0, 2);
            #region Продажи после прошлого Z-отчета
            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                //OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum, sum(vat7) as vat7, sum(vat10) as vat10, sum(vat15) as vat15, sum(vat20) as vat20 from " + FileName + " where type = '+' and date = '" + date + "' and time > '" + time + "'", connection);
                string datetime = date + time;
                OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum, sum(vat7) as vat7, sum(vat10) as vat10, sum(vat15) as vat15, sum(vat20) as vat20 from " + FileName + " where type = '+' and (date & time) > '" + datetime + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Пустой запрос");
                }
                else
                {
                    try
                    {
                        total += dt.Rows[0].Field<double>("total");
                        checkSum += dt.Rows[0].Field<int>("checksum");
                        VAT7 += dt.Rows[0].Field<double>("vat7");
                    }
                    catch { }
                    try
                    {
                        VAT10 += dt.Rows[0].Field<double>("vat10");
                    }
                    catch { }
                    try
                    {
                        VAT15 += dt.Rows[0].Field<double>("vat15");
                    }
                    catch { }
                    try
                    {
                        VAT20 += dt.Rows[0].Field<double>("vat20");
                    }
                    catch { }
                }

                connection.Close();
                connection.Dispose();
                command.Dispose();
                dt.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion Продажи после прошлого Z-отчета
            #region Возвраты после прошлого Z-отчета
            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                //OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum, sum(vat7) as vat7, sum(vat10) as vat10, sum(vat15) as vat15, sum(vat20) as vat20 from " + FileName + " where type = '-' and date = '" + date + "' and time > '" + time + "'", connection);
                string datetime = date + time;
                //OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum, sum(vat7) as vat7, sum(vat10) as vat10, sum(vat15) as vat15, sum(vat20) as vat20 from " + FileName + " where type = '-' and date + time > '" + datetime + "'", connection);
                OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum, sum(vat7) as vat7, sum(vat10) as vat10, sum(vat15) as vat15, sum(vat20) as vat20 from " + FileName + " where type = '-' and (date & time) > '" + datetime + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Пустой запрос");
                }
                else
                {
                    try
                    {
                        total -= dt.Rows[0].Field<double>("total");
                        checkSum += dt.Rows[0].Field<int>("checksum");
                        VAT7 -= dt.Rows[0].Field<double>("vat7");
                    }
                    catch { }
                    try
                    {
                        VAT10 -= dt.Rows[0].Field<double>("vat10");
                    }
                    catch { }
                    try
                    {
                        VAT15 -= dt.Rows[0].Field<double>("vat15");
                    }
                    catch { }
                    try
                    {
                        VAT20 -= dt.Rows[0].Field<double>("vat20");
                    }
                    catch { }
                }

                connection.Close();
                connection.Dispose();
                command.Dispose();
                dt.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion Возвраты после прошлого Z-отчета
        }

        public static void GetTotalAndCheckSumForXReport(string date, string time,
            out double total, out int checkSum,
            out double annulatedSum, out int annulatedCheckSum,
            out double returnSum, out int returnCheckSum,
            out double saleSum, out int saleCheckSum)
        {
            total = 0; checkSum = 0;
            saleSum = 0; saleCheckSum = 0;
            returnSum = 0; returnCheckSum = 0;
            annulatedSum = 0; annulatedCheckSum = 0;

            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Cheques";

            //string dateTime = DateTime.Now.ToString();
            //string dateString = dateTime.Substring(6, 4) + dateTime.Substring(3, 2) + dateTime.Substring(0, 2);

            #region сегодняшний день продажи
            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                //OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '+' and date = '" + date + "' and time > '" + time + "'", connection);
                OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '+' and date = '" + date + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Пустой запрос");
                }
                else
                {
                    try
                    {
                        total += dt.Rows[0].Field<double>("total");
                        saleSum += dt.Rows[0].Field<double>("total");
                    }
                    catch { }
                    try
                    {
                        checkSum += dt.Rows[0].Field<int>("checksum");
                        saleCheckSum += dt.Rows[0].Field<int>("checksum");
                    }
                    catch { }
                }

                connection.Close();
                connection.Dispose();
                command.Dispose();
                dt.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
            #region сегодняшний день возвраты
            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                //OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '-' and date = '" + date + "' and time > '" + time + "'", connection);
                OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '-' and date = '" + date + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Пустой запрос");
                }
                else
                {
                    try
                    {
                        total -= dt.Rows[0].Field<double>("total");
                        returnSum += dt.Rows[0].Field<double>("total");
                    }
                    catch { }
                    try
                    {
                        checkSum += dt.Rows[0].Field<int>("checksum");
                        returnCheckSum += dt.Rows[0].Field<int>("checksum");
                    }
                    catch { }
                }

                connection.Close();
                connection.Dispose();
                command.Dispose();
                dt.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
            #region прошлые дни продажи
            //try
            //{
            //    OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            //    connection.Open();
            //    OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '+' and date > '" + date + "'", connection);

            //    DataTable dt = new DataTable();
            //    dt.Load(command.ExecuteReader());

            //    if (dt.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Пустой запрос");
            //    }
            //    else
            //    {
            //        try
            //        {
            //            total += dt.Rows[0].Field<double>("total");
            //            saleSum += dt.Rows[0].Field<double>("total");
            //        }
            //        catch { }
            //        try
            //        {
            //            checkSum += dt.Rows[0].Field<int>("checksum");
            //            saleCheckSum += dt.Rows[0].Field<int>("checksum");
            //        }
            //        catch { }
            //    }

            //    connection.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //#endregion
            //#region прошлые дни возвраты
            //try
            //{
            //    OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            //    connection.Open();
            //    OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '-' and date > '" + date + "'", connection);

            //    DataTable dt = new DataTable();
            //    dt.Load(command.ExecuteReader());

            //    if (dt.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Пустой запрос");
            //    }
            //    else
            //    {
            //        try
            //        {
            //            total -= dt.Rows[0].Field<double>("total");
            //            returnSum += dt.Rows[0].Field<double>("total");
            //        }
            //        catch { }
            //        try
            //        {
            //            checkSum += dt.Rows[0].Field<int>("checksum");
            //            returnCheckSum += dt.Rows[0].Field<int>("checksum");
            //        }
            //        catch { }
            //    }

            //    connection.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            #endregion
            #region сегодняшний день аннуляции
            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                //OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '@' and date = '" + date + "' and time > '" + time + "'", connection);
                OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '@' and date = '" + date + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Пустой запрос");
                }
                else
                {
                    try
                    {
                        annulatedSum += dt.Rows[0].Field<double>("total");
                    }
                    catch { }
                    try
                    {
                        annulatedCheckSum += dt.Rows[0].Field<int>("checksum");
                    }
                    catch { }
                }

                connection.Close();
                connection.Dispose();
                command.Dispose();
                dt.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
            #region прошлые дни аннуляции
            //try
            //{
            //    OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            //    connection.Open();
            //    OleDbCommand command = new OleDbCommand("select sum(total) as total, count(*) as checksum from " + FileName + " where type = '@' and date > '" + date + "'", connection);

            //    DataTable dt = new DataTable();
            //    dt.Load(command.ExecuteReader());

            //    if (dt.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Пустой запрос");
            //    }
            //    else
            //    {
            //        try
            //        {
            //            annulatedSum += dt.Rows[0].Field<double>("total");
            //        }
            //        catch { }
            //        try
            //        {
            //            annulatedCheckSum += dt.Rows[0].Field<int>("checksum");
            //        }
            //        catch { }
            //    }

            //    connection.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            #endregion
        }
    }
}
