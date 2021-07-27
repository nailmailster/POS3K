using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace POS3K
{
    public class Fiscal
    {
        public Stream fStream;
        public BinaryReader reader;
        public BinaryWriter writer;
        public string datafile = Properties.Settings.Default.TAX;
        public string vatfile = Properties.Settings.Default.VAT;
        public FiscalHeader fiscalHeader = new FiscalHeader();
        public FiscalUser[] fiscalUser = new FiscalUser[9];
        public byte[] fd = new byte[40];
        public FiscalData fiscalData = new FiscalData();
        public VATData vATData = new VATData();
        public char[] emptyFiscalDate8 = new char[7];
        public char[] emptyFiscalDate9 = new char[9];

        public int currentUser = 0;

        public bool OpenRead()
        {
            try
            {
                fStream = File.OpenRead(datafile);
                reader = new BinaryReader(fStream, Encoding.Default);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public bool OpenWrite()
        {
            try
            {
                fStream = File.OpenWrite(datafile);
                writer = new BinaryWriter(fStream, Encoding.Default);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public bool SeekNewReg()
        {
            try
            {
                writer.Seek(2 + currentUser * 43, SeekOrigin.Begin);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public bool WriteNewReg(string ownerCode, string regCode, string fiscalDate, string fiscalTime)
        {
            try
            {
                writer.Write(ownerCode.ToCharArray(0, 13));
                writer.Write((char)0x0);
                writer.Write(regCode.ToCharArray(0, 12));
                writer.Write((char)0x0);
                writer.Write(fiscalDate.ToCharArray(0, 8));
                writer.Write((char)0x0);
                writer.Write(fiscalTime.ToCharArray(0, 6));
                if (currentUser != 8)
                    writer.Write((char)0x0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public bool Close()
        {
            try
            {
                fStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public bool GetHeader()
        {
            try
            {
                fiscalHeader.fiscalMode = reader.ReadChars(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public bool GetUsers()
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    fiscalUser[i] = new FiscalUser();
                    fiscalUser[i].regCode = reader.ReadChars(14);
                    fiscalUser[i].ownerCode = reader.ReadChars(13);
                    fiscalUser[i].fiscalDate = reader.ReadChars(9);
                    if (i != 8)
                        fiscalUser[i].fiscalTime = reader.ReadChars(7);
                    else
                        fiscalUser[i].fiscalTime = reader.ReadChars(6);
                    if (fiscalUser[i].fiscalDate.SequenceEqual(emptyFiscalDate8) || fiscalUser[i].fiscalDate.SequenceEqual(emptyFiscalDate9))
                    {
                        currentUser = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public long GetPosition()
        {
            return fStream.Position;
        }

        public bool GetNextZ()
        {
            fd = reader.ReadBytes(40);
            using (Stream fdStream = File.OpenWrite("fd.txt"))
            {
                BinaryWriter writer = new BinaryWriter(fdStream, Encoding.Default);
                writer.Write(fd);
                writer.Flush();
                fdStream.Close();
            }

            double total = DLLClass.ActivateDLL();
            FloatingPointReset.Action();

            string dateString = Encoding.Default.GetString(fd, 0, 8);
            string timeString = Encoding.Default.GetString(fd, 9, 6);

            int closeNumber = DLLClass.ActivateDLL2();
            FloatingPointReset.Action();

            int checkSum = DLLClass.ActivateDLL3();
            FloatingPointReset.Action();

            fiscalData.closeDate = dateString;
            fiscalData.closeTime = timeString;
            fiscalData.total = total;
            fiscalData.closeNumber = closeNumber;
            fiscalData.checkSum = checkSum;

            return true;
        }

        public bool GetLatestZ()
        {
            fStream.Position = fStream.Length - 40;
            fd = reader.ReadBytes(40);
            using (Stream fdStream = File.OpenWrite("fd.txt"))
            {
                BinaryWriter writer = new BinaryWriter(fdStream, Encoding.Default);
                writer.Write(fd);
                writer.Flush();
                fdStream.Close();
            }

            double total = 0;
            try
            {
                total = DLLClass.ActivateDLL();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DLLClass.ActivateDLL(): " + ex.Message);
            }
            try
            {
                FloatingPointReset.Action();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FloatingPointReset.Action(): " + ex.Message);
            }

            string dateString = Encoding.Default.GetString(fd, 0, 8);
            string timeString = Encoding.Default.GetString(fd, 9, 6);

            int closeNumber = DLLClass.ActivateDLL2();
            FloatingPointReset.Action();

            int checkSum = DLLClass.ActivateDLL3();
            FloatingPointReset.Action();

            fiscalData.closeDate = dateString;
            fiscalData.closeTime = timeString;
            fiscalData.total = total;
            fiscalData.closeNumber = closeNumber;
            fiscalData.checkSum = checkSum;

            return true;
        }

        public bool GetFiscalReportByDates(string reportType, string dateFrom, string dateTo)
        {
            if (reportType == "short")
            {
                using (StreamWriter writer = File.CreateText("queryds.dat"))
                {
                    writer.WriteLine(dateFrom);
                    writer.WriteLine(dateTo);
                }

                DLLClass.ActivateDLLRDS();
                FloatingPointReset.Action();
            }
            else if (reportType == "detailed")
            {
                using (StreamWriter writer = File.CreateText("querydd.dat"))
                {
                    writer.WriteLine(dateFrom);
                    writer.WriteLine(dateTo);
                }

                DLLClass.ActivateDLLRDD();
                FloatingPointReset.Action();
            }

            return true;
        }

        public bool GetFiscalReportByNumbers(string reportType, string numberFrom, string numberTo)
        {
            if (reportType == "short")
            {
                using (StreamWriter writer = File.CreateText("queryns.dat"))
                {
                    writer.WriteLine(numberFrom);
                    writer.WriteLine(numberTo);
                }

                DLLClass.ActivateDLLRNS();
                FloatingPointReset.Action();
            }
            else if (reportType == "detailed")
            {
                using (StreamWriter writer = File.CreateText("querynd.dat"))
                {
                    writer.WriteLine(numberFrom);
                    writer.WriteLine(numberTo);
                }

                DLLClass.ActivateDLLRND();
                FloatingPointReset.Action();
            }

            return true;
        }

        public int GetCloseNumber()
        {
            int closeNumber = DLLClass.ActivateDLL2();
            FloatingPointReset.Action();

            return closeNumber;
        }

        public int GetCheckSum()
        {
            int checkSum = DLLClass.ActivateDLL3();
            FloatingPointReset.Action();

            return checkSum;
        }

        public bool CompareDateTime()
        {
            DateTime dateTime = DateTime.Now;

            int zDay = Convert.ToInt32(fiscalData.closeDate.Substring(0, 2));
            int zMonth = Convert.ToInt32(fiscalData.closeDate.Substring(2, 2));
            int zYear = Convert.ToInt32(fiscalData.closeDate.Substring(4, 4));
            int zHour = Convert.ToInt32(fiscalData.closeTime.Substring(0, 2));
            int zMinute = Convert.ToInt32(fiscalData.closeTime.Substring(2, 2));
            int zSecond = Convert.ToInt32(fiscalData.closeTime.Substring(4, 2));

            DateTime zDateTime = new DateTime(zYear, zMonth, zDay, zHour, zMinute, zSecond);
            zDateTime = zDateTime.AddHours(24);

            int compareResult = dateTime.CompareTo(zDateTime);

            if (compareResult >= 0)
                return false;
            else
                return true;
        }

        public bool AddReport(string filePath)
        {
            bool dLLResult = false;

            if (!GetLatestZ())
                return false;
            int closeNumber = fiscalData.closeNumber;

            string zDate = fiscalData.closeDate.Substring(4, 4) + fiscalData.closeDate.Substring(2, 2) + fiscalData.closeDate.Substring(0, 2);
            string zTime = fiscalData.closeTime;

            ChequesDBF.GetTotalAndCheckSumAndVAT(zDate, zTime, out double total, out int checkSum, out double VAT7, out double VAT10, out double VAT15, out double VAT20);

            string dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            fiscalData.closeDate = dateTime.Substring(0, 2) + dateTime.Substring(3, 2) + dateTime.Substring(6, 4);
            fiscalData.closeTime = dateTime.Substring(11, 2) + dateTime.Substring(14, 2) + dateTime.Substring(17, 2);
            fiscalData.closeNumber++;
            fiscalData.total = total;
            fiscalData.checkSum = checkSum;

            vATData.VAT7 = VAT7;
            vATData.VAT10 = VAT10;
            vATData.VAT15 = VAT15;
            vATData.VAT20 = VAT20;

            using (StreamWriter writer = File.CreateText("temp.dat"))
            {
                writer.WriteLine(fiscalData.closeDate);
                writer.WriteLine(fiscalData.closeTime);
                writer.WriteLine(fiscalData.closeNumber.ToString());
                writer.WriteLine(fiscalData.total.ToString());
                writer.WriteLine(fiscalData.checkSum.ToString());

                writer.WriteLine(vATData.VAT7.ToString());
                writer.WriteLine(vATData.VAT10.ToString());
                writer.WriteLine(vATData.VAT15.ToString());
                writer.WriteLine(vATData.VAT20.ToString());
            }

            int tryOuts = 0;
            while (true)
            {
                tryOuts++;
                if (tryOuts > 100)
                    break;
                Close();
                Thread.Sleep(10);
                dLLResult = DLLClass.ActivateDLL7();
                Thread.Sleep(10);
                OpenRead();
                Thread.Sleep(10);
                GetLatestZ();
                Thread.Sleep(10);
                if (closeNumber < fiscalData.closeNumber)
                    break;
                else
                {
                    fiscalData.closeDate = dateTime.Substring(0, 2) + dateTime.Substring(3, 2) + dateTime.Substring(6, 4);
                    fiscalData.closeTime = dateTime.Substring(11, 2) + dateTime.Substring(14, 2) + dateTime.Substring(17, 2);
                    fiscalData.closeNumber++;
                    fiscalData.total = total;
                    fiscalData.checkSum = checkSum;

                    vATData.VAT7 = VAT7;
                    vATData.VAT10 = VAT10;
                    vATData.VAT15 = VAT15;
                    vATData.VAT20 = VAT20;

                    using (StreamWriter writer = File.CreateText("temp.dat"))
                    {
                        writer.WriteLine(fiscalData.closeDate);
                        writer.WriteLine(fiscalData.closeTime);
                        writer.WriteLine(fiscalData.closeNumber.ToString());
                        writer.WriteLine(fiscalData.total.ToString());
                        writer.WriteLine(fiscalData.checkSum.ToString());

                        writer.WriteLine(vATData.VAT7.ToString());
                        writer.WriteLine(vATData.VAT10.ToString());
                        writer.WriteLine(vATData.VAT15.ToString());
                        writer.WriteLine(vATData.VAT20.ToString());
                    }
                }
            }
            if (tryOuts > 1)
                MessageBox.Show("tryOuts == " + tryOuts.ToString());

            Close();

            if (dLLResult)
                Vars.lastZDateTime = fiscalData.closeDate.Substring(0, 2) + "." + fiscalData.closeDate.Substring(2, 2) + "." + fiscalData.closeDate.Substring(4, 4) + " " + fiscalData.closeTime.Substring(0, 2) + ":" + fiscalData.closeTime.Substring(2, 2) + ":" + fiscalData.closeTime.Substring(4, 2);

            return dLLResult;
        }
    }

    public class FiscalHeader
    {
        public char[] fiscalMode = new char[2];
    }

    public class FiscalUser
    {
        public char[] regCode = new char[14];
        public char[] ownerCode = new char[13];
        public char[] fiscalDate = new char[9];
        public char[] fiscalTime = new char[7];
    }

    public class FiscalData
    {
        public string closeDate;
        public string closeTime;
        public int closeNumber;
        public double total;
        public int checkSum;
    }

    public class VATData
    {
        public double VAT7;
        public double VAT10;
        public double VAT15;
        public double VAT20;
    }
}
