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
    public static class GoodsDBF
    {
        public static bool getRecordByBarcode(string barcode, ref ActivePosition activePosition, ref OleDbConnection connection)
        {
            Vars.goodsIsBusy = true;
            while (Vars.exchangeIsActive) ;
            //string FilePath = @"C:\Registan\POS\Bases\";
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Goods";
            long discount;

            try
            {
                //using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password="))
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        using (OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + barcode + "'", connection))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                dt.Load(command.ExecuteReader());

                                if (dt.Rows.Count == 0)
                                {
                                    if (barcode != barcode.ToLower())
                                    {
                                        command.CommandText = "select * from " + FileName + " where code = '" + barcode.ToLower() + "'";
                                        dt.Load(command.ExecuteReader());
                                        if (dt.Rows.Count == 0)
                                        {
                                            if (barcode != barcode.ToLower())
                                            {
                                                command.CommandText = "select * from " + FileName + " where code = '" + barcode.ToUpper() + "'";
                                                dt.Load(command.ExecuteReader());
                                            }
                                        }
                                    }
                                    else if (barcode.Length == 13 && barcode.Substring(0, 1) == "2")
                                    {
                                        command.CommandText = "select * from " + FileName + " where code like '" + barcode.Substring(0, 7) + "%'";
                                        dt.Load(command.ExecuteReader());
                                    }
                                }
                                //connection.Close();
                                //connection.Dispose();
                                command.Dispose();

                                if (dt.Rows.Count == 0)
                                {
                                    Vars.goodsIsBusy = false;
                                    return false;
                                }
                                else
                                {
                                    activePosition.Regime = ActivePositionModes.codeDone;
                                    activePosition.Barcode = dt.Rows[0].Field<string>("Code");
                                    activePosition.Description = dt.Rows[0].Field<string>("Name");
                                    activePosition.DescriptionLat = dt.Rows[0].Field<string>("Shortname");
                                    activePosition.Info = dt.Rows[0].Field<string>("Name");
                                    activePosition.Price = dt.Rows[0].Field<double>("Price");
                                    activePosition.IsWeight = dt.Rows[0].Field<bool>("Weight");
                                    try
                                    {
                                        activePosition.VATRate = (int)dt.Rows[0].Field<double>("Vat");
                                    }
                                    catch
                                    {
                                        activePosition.VATRate = 0;
                                    }
                                    if (activePosition.IsWeight && barcode.Length == 13)
                                    {
                                        activePosition.Quantity = Convert.ToInt32(barcode.Substring(7, 5));
                                    }
                                    bool isLoyal = dt.Rows[0].Field<bool>("Loyal");
                                    if (isLoyal)
                                        activePosition.PercentDiscount = dt.Rows[0].Field<double>("Ldiscount");

                                    //=====================
                                    //if (barcode != "21083853" && barcode != "21051913" && barcode != "21001789")
                                    if (barcode != "21001789")
                                        activePosition.VATRate = 15;
                                    //=====================

                                    Vars.goodsIsBusy = false;
                                    dt.Dispose();
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Vars.goodsIsBusy = false;
                return false;
            }
        }

        public static bool CheckAndUpdateOrAdd(string[] fields)
        {
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Goods";

            string barcode = fields[0];
            string longname = fields[1];
            string shortname = fields[2];
            string freeprice = fields[3];
            string price = fields[4];
            string weight = fields[5];
            string items = fields[6];
            string package = fields[7];
            string setqty = fields[8];
            string rest = fields[9];
            string lpricep = fields[10];
            string ldiscount = fields[11];
            string loyal = fields[12];
            string lprices = fields[13];
            string couponh = fields[14];
            string chdisco = fields[15];
            string chactive = fields[16];
            string vat = "";
            try
            {
                vat = fields[17];
            }
            catch(Exception)
            {
            }

            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + barcode + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    if (barcode != barcode.ToLower())
                    {
                        command.CommandText = "select * from " + FileName + " where code = '" + barcode.ToLower() + "'";
                        dt.Load(command.ExecuteReader());
                        if (dt.Rows.Count == 0)
                        {
                            if (barcode != barcode.ToLower())
                            {
                                command.CommandText = "select * from " + FileName + " where code = '" + barcode.ToUpper() + "'";
                                dt.Load(command.ExecuteReader());
                            }
                        }
                    }
                    else if (barcode.Length == 13 && barcode.Substring(0, 1) == "2")
                    {
                        command.CommandText = "select * from " + FileName + " where code like '" + barcode.Substring(0, 7) + "%'";
                        dt.Load(command.ExecuteReader());
                    }
                }
                connection.Close();

                if (dt.Rows.Count == 0)
                {
                    AddRecord(barcode, longname, shortname, weight, ldiscount, loyal, price, vat);
                }
                else
                {
                    UpdateRecord(barcode, longname, shortname, weight, ldiscount, loyal, price, vat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public static bool AddRecord(string barcode, string longname, string shortname, string weight, string ldiscount, string loyal, string price, string vat)
        {
            int result;

            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Goods";

            bool Weight, Loyal;
            double Ldiscount, Price, Vat;

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            //OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=dBase IV");
            //OleDbConnection connection = new OleDbConnection(@"Provider=vfpoledb.1;Data Source=" + FilePath);

            connection.Open();
            try
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT into " + FileName + " ([Code], [Name], [Shortname], [Weight], [Ldiscount], [Loyal], [Price], [Vat]) VALUES (@Code, @Name, @Shortname, @Weight, @Ldiscount, @Loyal, @Price, @Vat)";
                cmd.Parameters.AddWithValue("@Code", barcode);
                if (longname.Length > 25)
                    longname = longname.Substring(0, 25);
                //cmd.CommandText = "INSERT into " + FileName + " (Code, Name, Shortname, Rest, Free, Weight, Isset, Lprices, Lpricep, Ldiscount, Loyal, Sapid, Price, Vat) VALUES ('123', 'привет', 'здорово!', 0, 0, 0, 0, 0, 0, 0, 0, 'n', 123, 20)";
                cmd.Parameters.AddWithValue("@Name", longname);
                if (shortname.Length > 20)
                    shortname = shortname.Substring(0, 20);
                cmd.Parameters.AddWithValue("@Shortname", shortname);
                if (weight == "1")
                    Weight = true;
                else
                    Weight = false;
                cmd.Parameters.AddWithValue("@Weight", Weight);
                Ldiscount = Convert.ToDouble(ldiscount);
                cmd.Parameters.AddWithValue("@Ldiscount", Ldiscount);
                if (loyal == "1")
                    Loyal = true;
                else
                    Loyal = false;
                cmd.Parameters.AddWithValue("@Loyal", Loyal);
                Price = Convert.ToDouble(price);
                cmd.Parameters.AddWithValue("@Price", Price);
                if (vat == "")
                    Vat = 0;
                else if (vat == "НДС7")
                    Vat = 7;
                else if (vat == "НДС10")
                    Vat = 10;
                else if (vat == "НДС15")
                    Vat = 15;
                else if (vat == "НДС20")
                    Vat = 20;
                else if (vat == "БезНДС")
                    Vat = 0;
                else if (vat == "НДС0")
                    Vat = 0;
                else
                    Vat = 0;
                cmd.Parameters.AddWithValue("@Vat", Vat);
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
                result = 0;
            }
            connection.Close();

            if (result == 1)
                return true;
            else
                return false;
        }

        public static bool UpdateRecord(string barcode, string longname, string shortname, string weight, string ldiscount, string loyal, string price, string vat)
        {
            int result;

            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Goods";

            bool Weight, Loyal;
            double Ldiscount, Price, Vat = 0;

            OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            connection.Open();
            try
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                //OleDbCommand command = new OleDbCommand("update " + FileName + " set [sums] = @SumNew  where [code] = @Code", connection);
                cmd.CommandText = "UPDATE " + FileName + " SET [Name] = @Name, [Shortname] = @Shortname, [Weight] = @Weight, [Ldiscount] = @Ldiscount, [Loyal] = @Loyal, [Price] = @Price, [Vat] = @Vat where [Code] = @Code";
                cmd.Parameters.AddWithValue("@Code", barcode);
                if (longname.Length > 25)
                    longname = longname.Substring(0, 25);
                cmd.Parameters.AddWithValue("@Name", longname);
                if (shortname.Length > 20)
                    shortname = shortname.Substring(0, 20);
                cmd.Parameters.AddWithValue("@Shortname", shortname);
                if (weight == "1")
                    Weight = true;
                else
                    Weight = false;
                cmd.Parameters.AddWithValue("@Weight", Weight);
                Ldiscount = Convert.ToDouble(ldiscount);
                cmd.Parameters.AddWithValue("@Ldiscount", Ldiscount);
                if (loyal == "1")
                    Loyal = true;
                else
                    Loyal = false;
                cmd.Parameters.AddWithValue("@Loyal", Loyal);
                Price = Convert.ToDouble(price);
                cmd.Parameters.AddWithValue("@Price", Price);
                if (vat == "НДС7")
                    Vat = 7;
                else if (vat == "НДС10")
                    Vat = 10;
                else if (vat == "НДС15")
                    Vat = 15;
                else if (vat == "НДС20")
                    Vat = 20;
                else if (vat == "НДС0")
                    Vat = 0;
                else if (vat == "БезНДС")
                    Vat = 0;
                cmd.Parameters.AddWithValue("@Vat", Vat);
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
                result = 0;
            }
            connection.Close();

            if (result == 1)
                return true;
            else
                return false;
        }

        public static string getDescriptionByBarcode(string barcode)
        {
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Goods";

            try
            {
                OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                OleDbCommand command = new OleDbCommand("select name from " + FileName + " where code = '" + barcode + "'", connection);

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    if (barcode != barcode.ToLower())
                    {
                        command.CommandText = "select name from " + FileName + " where code = '" + barcode.ToLower() + "'";
                        dt.Load(command.ExecuteReader());
                        if (dt.Rows.Count == 0)
                        {
                            if (barcode != barcode.ToLower())
                            {
                                command.CommandText = "select name from " + FileName + " where code = '" + barcode.ToUpper() + "'";
                                dt.Load(command.ExecuteReader());
                            }
                        }
                    }
                    else if (barcode.Length == 13 && barcode.Substring(0, 1) == "2")
                    {
                        command.CommandText = "select name from " + FileName + " where code like '" + barcode.Substring(0, 7) + "%'";
                        dt.Load(command.ExecuteReader());
                    }
                }
                connection.Close();

                if (dt.Rows.Count == 0)
                {
                    return "НЕ НАЙДЕН";
                }
                else
                {
                    return dt.Rows[0].Field<string>("Name");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "ОШИБКА БД";
            }
        }

        public static bool CheckAndUpdateOrAddWholeDirectory(OleDbConnection exchangeConnection)
        {
            string FilePath = Properties.Settings.Default.DBF;
            string FileName = "Goods";
            string s;

            bool Weight, Loyal;
            double Ldiscount, Price, Vat = 0;

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


            //OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
            //connection.Open();
            if (exchangeConnection.State != ConnectionState.Open)
                exchangeConnection.Open();

            string[] files = Directory.GetFiles(Properties.Settings.Default.InPath);
            foreach (string file in files)
            {
                if (file.Substring(file.Length - 3, 3) == "loy" || file.Substring(file.Length - 3, 3) == "lck")
                {
                    continue;
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.Trim().Length == 0)
                        continue;
                    if (fileInfo.Length > 1024 * 100)
                        continue;
                    if (Vars.goodsIsBusy)
                    {
                        //connection.Close();
                        Vars.exchangeIsActive = false;
                        return false;
                    }
                    using (StreamReader reader = new StreamReader(file, encoding: Encoding.Default))
                    {
                        if (Vars.goodsIsBusy)
                        {
                            //connection.Close();
                            Vars.exchangeIsActive = false;
                            return false;
                        }
                        while (!reader.EndOfStream)
                        {
                            if (Vars.goodsIsBusy)
                            {
                                //connection.Close();
                                Vars.exchangeIsActive = false;
                                return false;
                            }
                            s = reader.ReadLine();
                            string[] fields = s.Split(';');
                            string barcode = fields[0];
                            //if (barcode == "4607141332302")
                            //    MessageBox.Show("4607141332302");
                            string longname = fields[1];
                            string shortname = fields[2];
                            string freeprice = fields[3];
                            string price = fields[4];
                            string weight = fields[5];
                            string items = fields[6];
                            string package = fields[7];
                            string setqty = fields[8];
                            string rest = fields[9];
                            string lpricep = fields[10];
                            string ldiscount = fields[11];
                            string loyal = fields[12];
                            string lprices = fields[13];
                            string couponh = fields[14];
                            string chdisco = fields[15];
                            string chactive = fields[16];
                            string vat = "";
                            try
                            {
                                vat = fields[17];
                            }
                            catch (Exception)
                            {
                            }

                            try
                            {
                                if (Vars.goodsIsBusy)
                                {
                                    //connection.Close();
                                    Vars.exchangeIsActive = false;
                                    return false;
                                }
                                //using (OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + barcode + "'", connection))
                                using (OleDbCommand command = new OleDbCommand("select * from " + FileName + " where code = '" + barcode + "'", exchangeConnection))
                                {
                                    using (DataTable dt = new DataTable())
                                    {
                                        dt.Load(command.ExecuteReader());

                                        if (dt.Rows.Count == 0)
                                        {
                                            if (barcode != barcode.ToLower())
                                            {
                                                if (Vars.goodsIsBusy)
                                                {
                                                    //connection.Close();
                                                    Vars.exchangeIsActive = false;
                                                    return false;
                                                }
                                                command.CommandText = "select * from " + FileName + " where code = '" + barcode.ToLower() + "'";
                                                dt.Load(command.ExecuteReader());
                                                if (dt.Rows.Count == 0)
                                                {
                                                    if (barcode != barcode.ToLower())
                                                    {
                                                        command.CommandText = "select * from " + FileName + " where code = '" + barcode.ToUpper() + "'";
                                                        dt.Load(command.ExecuteReader());
                                                    }
                                                }
                                            }
                                            else if (barcode.Length == 13 && barcode.Substring(0, 1) == "2")
                                            {
                                                if (Vars.goodsIsBusy)
                                                {
                                                    //connection.Close();
                                                    Vars.exchangeIsActive = false;
                                                    return false;
                                                }
                                                command.CommandText = "select * from " + FileName + " where code like '" + barcode.Substring(0, 7) + "%'";
                                                dt.Load(command.ExecuteReader());
                                            }
                                        }

                                        if (dt.Rows.Count == 0) //  Add
                                        {
                                            try
                                            {
                                                if (Vars.goodsIsBusy)
                                                {
                                                    //connection.Close();
                                                    Vars.exchangeIsActive = false;
                                                    return false;
                                                }
                                                OleDbCommand cmd = new OleDbCommand();
                                                //cmd.Connection = connection;
                                                cmd.Connection = exchangeConnection;
                                                cmd.CommandType = CommandType.Text;
                                                cmd.CommandText = "INSERT into " + FileName + " ([Code], [Name], [Shortname], [Weight], [Ldiscount], [Loyal], [Price], [Vat]) VALUES (@Code, @Name, @Shortname, @Weight, @Ldiscount, @Loyal, @Price, @Vat)";
                                                cmd.Parameters.AddWithValue("@Code", barcode);
                                                if (longname.Length > 25)
                                                    longname = longname.Substring(0, 25);
                                                cmd.Parameters.AddWithValue("@Name", longname);
                                                if (shortname.Length > 20)
                                                    shortname = shortname.Substring(0, 20);
                                                cmd.Parameters.AddWithValue("@Shortname", shortname);
                                                if (weight == "1")
                                                    Weight = true;
                                                else
                                                    Weight = false;
                                                cmd.Parameters.AddWithValue("@Weight", Weight);
                                                Ldiscount = Convert.ToDouble(ldiscount);
                                                cmd.Parameters.AddWithValue("@Ldiscount", Ldiscount);
                                                if (loyal == "1")
                                                    Loyal = true;
                                                else
                                                    Loyal = false;
                                                cmd.Parameters.AddWithValue("@Loyal", Loyal);
                                                Price = Convert.ToDouble(price);
                                                cmd.Parameters.AddWithValue("@Price", Price);
                                                if (vat == "")
                                                    Vat = 0;
                                                else if (vat == "НДС7")
                                                    Vat = 7;
                                                else if (vat == "НДС10")
                                                    Vat = 10;
                                                else if (vat == "НДС15")
                                                    Vat = 15;
                                                else if (vat == "НДС20")
                                                    Vat = 20;
                                                else if (vat == "НДС0")
                                                    Vat = 0;
                                                else if (vat == "БезНДС")
                                                    Vat = 0;
                                                else
                                                    Vat = 0;
                                                cmd.Parameters.AddWithValue("@Vat", Vat);
                                                if (Vars.goodsIsBusy)
                                                {
                                                    //connection.Close();
                                                    Vars.exchangeIsActive = false;
                                                    return false;
                                                }
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
                                                    //connection.Close();
                                                    Vars.exchangeIsActive = false;
                                                    return false;
                                                }
                                                OleDbCommand cmd = new OleDbCommand();
                                                //cmd.Connection = connection;
                                                cmd.Connection = exchangeConnection;
                                                cmd.CommandType = CommandType.Text;
                                                //OleDbCommand command = new OleDbCommand("update " + FileName + " set [sums] = @SumNew  where [code] = @Code", connection);
                                                cmd.CommandText = "UPDATE " + FileName + " SET [Name] = @NameArg, [Shortname] = @ShortnameArg, [Weight] = @WeightArg, [Ldiscount] = @LdiscountArg, [Loyal] = @LoyalArg, [Price] = @PriceArg, [Vat] = @VatArg where [Code] = @CodeArg";
                                                if (longname.Length > 25)
                                                    longname = longname.Substring(0, 25);
                                                cmd.Parameters.AddWithValue("@NameArg", longname);
                                                if (shortname.Length > 20)
                                                    shortname = shortname.Substring(0, 20);
                                                cmd.Parameters.AddWithValue("@ShortnameArg", shortname);
                                                if (weight == "1")
                                                    Weight = true;
                                                else
                                                    Weight = false;
                                                cmd.Parameters.AddWithValue("@WeightArg", Weight);
                                                Ldiscount = Convert.ToDouble(ldiscount);
                                                cmd.Parameters.AddWithValue("@LdiscountArg", Ldiscount);
                                                if (loyal == "1")
                                                    Loyal = true;
                                                else
                                                    Loyal = false;
                                                cmd.Parameters.AddWithValue("@LoyalArg", Loyal);
                                                Price = Convert.ToDouble(price);
                                                cmd.Parameters.AddWithValue("@PriceArg", Price);
                                                if (vat == "НДС7")
                                                    Vat = 7;
                                                else if (vat == "НДС10")
                                                    Vat = 10;
                                                else if (vat == "НДС15")
                                                    Vat = 15;
                                                else if (vat == "НДС20")
                                                    Vat = 20;
                                                else if (vat == "БезНДС")
                                                    Vat = 0;
                                                else if (vat == "НДС0")
                                                    Vat = 0;
                                                cmd.Parameters.AddWithValue("@VatArg", Vat);
                                                cmd.Parameters.AddWithValue("@CodeArg", barcode);
                                                if (Vars.goodsIsBusy)
                                                {
                                                    //connection.Close();
                                                    Vars.exchangeIsActive = false;
                                                    return false;
                                                }
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
                    //connection.Close();
                    Vars.exchangeIsActive = false;
                    return false;
                }
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File.Delete error: " + file);
                }
            }
            //connection.Close();
            Vars.exchangeIsActive = false;
            return true;
        }
    }
}
