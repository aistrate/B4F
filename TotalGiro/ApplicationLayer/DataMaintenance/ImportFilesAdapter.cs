using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Communicator.Currencies;
//using B4F.TotalGiro.Communicator.TextFiles;
using B4F.TotalGiro.Communicator.TBM;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class ImportFilesAdapter
    {
        public static DateTime GetMaxHistoricalExRateDate()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DateTime date = HistoricalExRateMapper.GetMaxHistoricalExRateDate(session);
            session.Close();
            return date;
        }

        public static void ImportExchangeRates(DateTime startDate)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ECBRates rates = new ECBRates();
            rates.LoadExRates(session, startDate);
            session.Close();
        }

        public static void ImportBackOfficeFiles()
        {
            //IDalSession session = NHSessionFactory.CreateSession();
            //IList files = TextFileToImportMapper.GetTextFilesToImport(session);
            //SqlConnection TheConn = (new B4F.TotalGiro.Dal.AdoNet.AdoDataConnect(session, 200)).CurrentConnection;
            //foreach (TextFileToImport tfi in files)
            //{
            //    tfi.SqlConn = TheConn;
            //    IList importedFiles = tfi.ImportAllFiles();
            //}

            ////foreach (TextFileToImport tfi in files)
            ////{
            ////    IList importedFiles = tfi.ImportAllFiles();
            ////    TextFileToImportMapper.Update(session, importedFiles);
            ////}

            //session.Close();
        }

        public static void MapExternalDataFromTextFiles()
        {
            //IDalSession session = NHSessionFactory.CreateSession();
            //TextFileToImportMapper.RunAllMappingSP(session);
            //session.Close();
        }

        public static void ImportHistoricalPrices()
        {
            TBMRequest tbmrequest = new TBMRequest();
            DataSet ds = TBMDataAdapter.GetMutualFunds();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string isin = "";

                if (dr.ItemArray[1].GetType().ToString() != "System.DBNull")
                {
                    isin = (string)dr.ItemArray[1];

                    tbmrequest.updateQuoteHistory(isin);
                }
            }
        }

        public static void ImportPdfReceiptsFromBo()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            // Read in all PDF files from the input directory for transaction receipts
            foreach (string filename in Directory.GetFiles("c:\\temp\\pdf", "*.pdf"))
            {
                FileInfo fi = new FileInfo(filename);
                FileStream fs = null;

                try
                {
                    // Filename format is EGVL000000-yymmdd.pdf
                    string[] arrFilenameParts = fi.Name.Substring(0, fi.Name.IndexOf('.')).Split('-');

                    string filedate = arrFilenameParts[1];
                    string accountnumber = arrFilenameParts[0];

                    IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccountByNumber(session, accountnumber);

                    TransactionReceipt tr = new TransactionReceipt();
                    tr.GiroAccount = account;
                    tr.CreationDate = DateTime.Now;
                    tr.FileDate = fi.CreationTime;
                    int nday = int.Parse(filedate.Substring(4, 2));
                    int nmonth = int.Parse(filedate.Substring(2, 2));
                    int nyear = int.Parse(string.Concat("20", filedate.Substring(0, 2)));
                    tr.ReceiptDate = new DateTime(nyear, nmonth, nday);
                    tr.FileSize = fi.Length;
                    tr.FileName = fi.Name;

                    // Copy content of the file to the byte array
                    fs = File.OpenRead(filename);

                    tr.FileContent = readBinFile(fs);

                    session.Insert(tr);

                    fs.Close();

                    // Now that we created the pdf in the database for safekeeping

                    // Move the file to the DONE folder
                    //fi.MoveTo("c:\\temp\\pdf\\done\\" + fi.Name);
                    // Maybe mail the file to the client

                    // Get the email address of the client
                    string email = "";
                    IList contactlist = ContactMapper.GetContacts(session, null, accountnumber, string.Empty, false, false);

                    if (contactlist.Count > 0)
                    {
                        //          email = ((Contact)contactlist[0]).ContactDetails.Email;
                    }

                    MailMessage msg = new MailMessage("steven.kribbe@bits4finance.com", "steven.kribbe@bits4finance.com");
                    msg.Subject = "Transaction receipt from Vierlander";
                    msg.From = new MailAddress("steven.kribbe@bits4finance.com");

                    //msg.Attachments.Add(new Attachment(fi.FullName));

                    SmtpClient smtpclient = new SmtpClient("honey.borghols.local", 25);
                    smtpclient.UseDefaultCredentials = false;

                    smtpclient.Credentials = new NetworkCredential("BORGHOLS\\steven.kribbe", "Bits5555");

                    smtpclient.Send(msg);

                }
                catch (Exception ex)
                {
                    // Something went wrong saving the content of the file to the database
                    // Ignore this error and continue with the other files
                    ex = ex;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }
            session.Close();
        }

        private static byte[] readBinFile(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}
