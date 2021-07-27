using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.IO;

namespace POS3K
{
    public static class LoyaltyDBF
    {
        public static bool GetRecordByCode(ref Loyalty loyalty)
        {
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Loyalty";

            loyalty.Banned = false;
            loyalty.Activated = false;
            loyalty.Sums = 0;
            loyalty.Points = 0;
            loyalty.Percent = 0;
            loyalty.FIO = "";
            loyalty.FIOLat = "";
            loyalty.ChSums = 0;
            loyalty.ChPerc = 0;

            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + loyalty.Code + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                connection.Close();

                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    loyalty.Banned = dt.Rows[0].Field<bool>("Banned");
                    loyalty.Activated = dt.Rows[0].Field<bool>("Activated");
                    try
                    {
                        loyalty.Sums = dt.Rows[0].Field<double>("Sums");
                    }
                    catch { };
                    try
                    {
                        loyalty.Points = (int)dt.Rows[0].Field<double>("Points");
                    }
                    catch { };
                    try
                    {
                        loyalty.Percent = dt.Rows[0].Field<double>("Percent");
                    }
                    catch { };
                    loyalty.FIO = dt.Rows[0].Field<string>("Fio");
                    loyalty.FIOLat = dt.Rows[0].Field<string>("Fiolat");
                    try
                    {
                        loyalty.ChSums = dt.Rows[0].Field<double>("Chsums");
                    }
                    catch { };
                    try
                    {
                        loyalty.ChPerc = dt.Rows[0].Field<double>("Chperc");
                    }
                    catch { };

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool UpdateSums(ref Loyalty loyalty)
        {
            return true;

            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Loyalty";

            int result;

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();
            try
            {
                OleDbCommand command = new OleDbCommand("update " + FileName + " set [sums] = @SumNew  where [code] = @Code", connection);
                command.Parameters.AddWithValue("@SumNew", loyalty.SumNew);
                command.Parameters.AddWithValue("@Code", loyalty.Code);

                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
                return false;
            }
            connection.Close();

            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool UpdateFromLoys(string directory)
        {
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Loyalty";
            string s;

            int result;

            if (Vars.autonomode)
            {
                Vars.exchangeIsActive = false;
                return false;
            }

            if (Vars.goodsIsBusy)
            {
                Vars.exchangeIsActive = false;
                return false;
            }

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();

            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                if (Vars.goodsIsBusy)
                {
                    connection.Close();
                    Vars.exchangeIsActive = false;
                    return false;
                }
                if (file.Substring(file.Length - 3, 3) != "loy")
                {
                    continue;
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.Trim().Length == 0)
                        continue;
                    if (fileInfo.Length > 1024 * 15)
                        continue;
                    using (StreamReader reader = new StreamReader(file, encoding: Encoding.Default))
                    {
                        if (Vars.goodsIsBusy)
                        {
                            connection.Close();
                            Vars.exchangeIsActive = false;
                            return false;
                        }
                        s = reader.ReadLine();
                        string[] fields = s.Split(';');
                        string id = fields[0];
                        string cardcode = fields[1];
                        string dn = fields[2];
                        string qty = fields[3];
                        string price = fields[4];
                        string sum = fields[5];
                        string sumAdd = fields[6];
                        string cashier = fields[7];

                        string sumSub = "";

                        while (!reader.EndOfStream)
                        {
                            if (Vars.goodsIsBusy)
                            {
                                connection.Close();
                                Vars.exchangeIsActive = false;
                                return false;
                            }
                            s = reader.ReadLine();
                            fields = s.Split(';');

                            string positionNumber = fields[0];
                            string barcode = fields[1];
                            string description = fields[2];
                            qty = fields[3];
                            price = fields[4];
                            sum = fields[5];

                            if (barcode == "loyalpayment")
                            {
                                sumSub = fields[5];
                                break;
                            }
                            else if (barcode == "endofcheque")
                            {
                                break;
                            }
                        }

                        try
                        {
                            if (Vars.goodsIsBusy)
                            {
                                connection.Close();
                                Vars.exchangeIsActive = false;
                                return false;
                            }
                            OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + cardcode + "'", connection);

                            DataTable dt = new DataTable();
                            dt.Load(command.ExecuteReader());

                            if (dt.Rows.Count == 0) //  Add
                            {
                                try
                                {
                                    OleDbCommand cmd = new OleDbCommand();
                                    cmd.Connection = connection;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = "INSERT into " + FileName + " ([Code], [Banned], [Activated], [Sums], [Points], [Percent], [Fio], [Fiolat]) VALUES (@Code, @Banned, @Activated, @Sums, @Points, @Percent, @Fio, @Fiolat)";
                                    cmd.Parameters.AddWithValue("@Code", cardcode);
                                    cmd.Parameters.AddWithValue("@Banned", false);
                                    cmd.Parameters.AddWithValue("@Activated", true);
                                    cmd.Parameters.AddWithValue("@Sums", Convert.ToDouble(sumAdd) - Convert.ToDouble(sumSub));
                                    cmd.Parameters.AddWithValue("@Points", 0);
                                    cmd.Parameters.AddWithValue("@Fio", "");
                                    cmd.Parameters.AddWithValue("@Fiolat", "");
                                    result = cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //connection.Close();
                                    //MessageBox.Show(ex.Message);
                                    result = 0;
                                }
                            }
                            else//  Update
                            {
                                if (Vars.goodsIsBusy)
                                {
                                    connection.Close();
                                    Vars.exchangeIsActive = false;
                                    return false;
                                }
                                try
                                {
                                    double sumToAdd = Convert.ToDouble(sumAdd);
                                    if (sumSub == "")
                                        sumSub = "0";
                                    double sumToSub = Convert.ToDouble(sumSub);
                                    double oldSum = dt.Rows[0].Field<double>("Sums");

                                    OleDbCommand cmd = new OleDbCommand();
                                    cmd.Connection = connection;
                                    cmd.CommandType = CommandType.Text;
                                    //OleDbCommand command = new OleDbCommand("update " + FileName + " set [sums] = @SumNew  where [code] = @Code", connection);
                                    cmd.CommandText = "UPDATE " + FileName + " SET [Sums] = @SumsOle where [Code] = @CodeOle";
                                    cmd.Parameters.AddWithValue("@SumsOle", oldSum + sumToAdd - sumToSub);
                                    cmd.Parameters.AddWithValue("@CodeOle", cardcode);
                                    result = cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    //connection.Close();
                                    //MessageBox.Show(ex.Message);
                                    result = 0;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);
                            //return false;
                        }
                    }
                }
                if (Vars.goodsIsBusy)
                {
                    connection.Close();
                    Vars.exchangeIsActive = false;
                    return false;
                }
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    MessageBox.Show("File.Delete error: " + file);
                }
            }

            connection.Close();
            return true;
        }

        public static bool UpdateFromLyts(string directory)
        {
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Loyalty";
            string s;

            int result;

            if (Vars.autonomode)
            {
                Vars.exchangeIsActive = false;
                return false;
            }

            if (Vars.goodsIsBusy)
            {
                Vars.exchangeIsActive = false;
                return false;
            }

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();

            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                if (Vars.goodsIsBusy)
                {
                    connection.Close();
                    Vars.exchangeIsActive = false;
                    return false;
                }
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension != "lyt")
                    continue;
                else
                {
                    //FileInfo fileInfo = new FileInfo(file);
                    //if (fileInfo.Length > 1024 * 15)
                    //    continue;
                    using (StreamReader reader = new StreamReader(file, encoding: Encoding.Default))
                    {
                        if (Vars.goodsIsBusy)
                        {
                            connection.Close();
                            Vars.exchangeIsActive = false;
                            return false;
                        }
                        while (!reader.EndOfStream)
                        {
                            s = reader.ReadLine();
                            string[] fields = s.Split(';');
                            if (fields.Length < 10)
                                continue;

                            string cardcode = fields[0];
                            string active = fields[1];
                            string fio = fields[2];
                            string fioLat = fields[3];
                            string points = fields[4];
                            string sum = fields[5];
                            string percent = fields[6];
                            string banned = fields[7];
                            string charityPercent = fields[8];
                            string charitySum = fields[9];

                            bool _active, _banned;
                            double _sum = 0;

                            if (active == "1")
                                _active = true;
                            else
                                _active = false;

                            if (banned == "1")
                                _banned = true;
                            else
                                _banned = false;

                            try
                            {
                                _sum = Convert.ToDouble(sum);
                            }
                            catch { }

                            if (Vars.goodsIsBusy)
                            {
                                connection.Close();
                                Vars.exchangeIsActive = false;
                                return false;
                            }
                            try
                            {
                                OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + cardcode + "'", connection);

                                DataTable dt = new DataTable();
                                dt.Load(command.ExecuteReader());

                                if (dt.Rows.Count == 0) //  Add
                                {
                                    try
                                    {
                                        if (Vars.goodsIsBusy)
                                        {
                                            connection.Close();
                                            Vars.exchangeIsActive = false;
                                            return false;
                                        }
                                        OleDbCommand cmd = new OleDbCommand();
                                        cmd.Connection = connection;
                                        cmd.CommandType = CommandType.Text;
                                        cmd.CommandText = "INSERT into " + FileName + " ([Code], [Banned], [Activated], [Sums], [Points], [Percent], [Fio], [Fiolat]) VALUES (@Code, @Banned, @Activated, @Sums, @Points, @Percent, @Fio, @Fiolat)";
                                        cmd.Parameters.AddWithValue("@Code", cardcode);
                                        cmd.Parameters.AddWithValue("@Banned", _banned);
                                        cmd.Parameters.AddWithValue("@Activated", _active);
                                        cmd.Parameters.AddWithValue("@Sums", _sum);
                                        cmd.Parameters.AddWithValue("@Points", 0);
                                        cmd.Parameters.AddWithValue("@Fio", fio);
                                        cmd.Parameters.AddWithValue("@Fiolat", fioLat);
                                        result = cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        //connection.Close();
                                        //MessageBox.Show(ex.Message);
                                        result = 0;
                                    }
                                }
                                else//  Update
                                {
                                    try
                                    {
                                        if (Vars.goodsIsBusy)
                                        {
                                            connection.Close();
                                            Vars.exchangeIsActive = false;
                                            return false;
                                        }
                                        OleDbCommand cmd = new OleDbCommand();
                                        cmd.Connection = connection;
                                        cmd.CommandType = CommandType.Text;
                                        //OleDbCommand command = new OleDbCommand("update " + FileName + " set [sums] = @SumNew  where [code] = @Code", connection);
                                        cmd.CommandText = "UPDATE " + FileName + " SET [Sums] = @SumsOle where [Code] = @CodeOle";
                                        cmd.Parameters.AddWithValue("@SumsOle", _sum);
                                        cmd.Parameters.AddWithValue("@CodeOle", cardcode);
                                        result = cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        //connection.Close();
                                        //MessageBox.Show(ex.Message);
                                        result = 0;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message);
                                //return false;
                            }
                        }
                    }
                }
                if (Vars.goodsIsBusy)
                {
                    connection.Close();
                    Vars.exchangeIsActive = false;
                    return false;
                }
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    MessageBox.Show("File.Delete error: " + file);
                }
            }

            connection.Close();
            return true;
        }
    }
}
