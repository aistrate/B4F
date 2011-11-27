using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Reports.Documents;
using B4F.TotalGiro.Reports.Notas;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;


namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public static class PrintNotasAdapter
    {
        #region Retrieve Data

        public static void GetCurrentManagmentCompany(out int id, out string name)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company != null)
                {
                    id = company.Key;
                    name = company.CompanyName;
                }
                else
                    throw new ApplicationException("Could not find current management company.");
            }
            finally
            {
                session.Close();
            }
        }

        #endregion

        #region Creating Notas

        public static void CreateNotas(BatchExecutionResults results)
        {
            CreateNotas(results, 0, 0);
        }

        public static void CreateNotas(BatchExecutionResults results, int managementCompanyId)
        {
            CreateNotas(results, managementCompanyId, 0);
        }

        public static void CreateNotas(BatchExecutionResults results, int managementCompanyId, int accountId)
        {
            checkCompanyAndAccount(managementCompanyId, accountId);

            CreateNotasFromOrders(results, managementCompanyId, accountId);

            // Which transaction types should be processed, and in what order
            TransactionTypes[] transactionTypes = new TransactionTypes[] {
                    TransactionTypes.NTM,   // NotaTransfer
                    TransactionTypes.InstrumentConversion     // NotaCorporateAction
                };

            foreach (TransactionTypes transactionType in transactionTypes)
                CreateNotasFromTransactions(results, transactionType, managementCompanyId, accountId);

            // Which booking types should be processed, and in what order
            GeneralOperationsBookingReturnClass[] bookingTypes = new GeneralOperationsBookingReturnClass[] {
                    GeneralOperationsBookingReturnClass.ManagementFee,  // NotaFees
                    GeneralOperationsBookingReturnClass.CashDividend,    // NotaDividend
                    GeneralOperationsBookingReturnClass.CashTransfer    // NotaDeposit
                };

            foreach (GeneralOperationsBookingReturnClass bookingType in bookingTypes)
                CreateNotasFromBookings(results, bookingType, managementCompanyId, accountId);
        }

        private static void checkCompanyAndAccount(int managementCompanyId, int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                if (managementCompanyId != 0)
                {
                    IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
                    if (managementCompany == null)
                        throw new ApplicationException(string.Format("ManagementCompany {0} not found.", managementCompanyId));
                }

                if (accountId != 0)
                {
                    ICustomerAccount account = AccountMapper.GetAccount(session, accountId) as ICustomerAccount;
                    if (account == null)
                        throw new ApplicationException(string.Format("Customer Account {0} not found.", accountId));
                }
            }
            finally
            {
                session.Close();
            }
        }

        public static void CreateNotasFromOrders(BatchExecutionResults results, int managementCompanyId, int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            int[] notarizableOrderIds = null;

            try
            {
                notarizableOrderIds = OrderMapper.GetNotarizableOrderIds(session, managementCompanyId, accountId);
            }
            catch (Exception ex)
            {
                results.MarkError(new ApplicationException("Error retrieving notarizable orders.", ex));
            }
            finally
            {
                session.Close();
            }

            foreach (int orderId in notarizableOrderIds)
                CreateNotaFromOrder(results, orderId);
        }

        public static void CreateNotaFromOrder(BatchExecutionResults results, int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                ISecurityOrder order = OrderMapper.GetOrder(session, orderId) as ISecurityOrder;
                if (order != null)
                {
                    INota nota = order.CreateNota();
                    if (nota != null)
                    {
                        NotaMapper.Update(session, nota);
                        results.MarkSuccess();
                    }
                }
                else
                    results.MarkError(new ApplicationException(string.Format("Security Order {0} not found.", orderId)));
            }
            catch (Exception ex)
            {
                results.MarkError(new ApplicationException(string.Format("Error creating nota for order {0}.", orderId), ex));
            }
            finally
            {
                session.Close();
            }
        }

        public static void CreateNotasFromTransactions(BatchExecutionResults results, TransactionTypes transactionType, int managementCompanyId, int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            int[] notarizableTxIds = null;

            try
            {
                notarizableTxIds = TransactionMapper.GetNotarizableTransactionIds(session, transactionType, managementCompanyId, accountId);
            }
            catch (Exception ex)
            {
                results.MarkError(
                    new ApplicationException(string.Format("Error retrieving notarizable transactions ({0}s).", transactionType), ex));
            }
            finally
            {
                session.Close();
            }

            foreach (int transactionId in notarizableTxIds)
                createNotaFromTransaction(results, transactionId, transactionType);
        }

        public static void CreateNotaFromTransaction(BatchExecutionResults results, int transactionId)
        {
            createNotaFromTransaction(results, transactionId, TransactionTypes.Transaction);
        }

        private static void createNotaFromTransaction(BatchExecutionResults results, int transactionId, TransactionTypes transactionType)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                ITransaction transaction = TransactionMapper.GetTransaction(session, transactionId);
                if (transaction != null)
                {
                    INota nota = transaction.CreateNota();
                    if (nota != null)
                    {
                        NotaMapper.Update(session, nota);
                        results.MarkSuccess();
                    }
                    else
                        // in case NotaMigrated was set to 'true' (e.g. for a storno of a Monetary Order transaction)
                        TransactionMapper.Update(session, transaction);
                }
                else
                    results.MarkError(new ApplicationException(string.Format("Transaction {0} not found.", transactionId)));
            }
            catch (Exception ex)
            {
                results.MarkError(new ApplicationException(string.Format("Error creating nota for transaction {0} ({1}).",
                                                                                       transactionId, transactionType), ex));
            }
            finally
            {
                session.Close();
            }
        }

        public static void CreateNotasFromBookings(BatchExecutionResults results, GeneralOperationsBookingReturnClass bookingType, int managementCompanyId, int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            int[] notarizableBookingIds = null;

            try
            {
                notarizableBookingIds = GeneralOperationsBookingMapper.GetNotarizableBookingIds(session, bookingType, managementCompanyId, accountId);
            }
            catch (Exception ex)
            {
                results.MarkError(
                    new ApplicationException(string.Format("Error retrieving notarizable bookings ({0}s).", bookingType), ex));
            }
            finally
            {
                session.Close();
            }

            foreach (int bookingId in notarizableBookingIds)
                createNotaFromBooking(results, bookingId, bookingType);
        }

        public static void CreateNotasFromBooking(BatchExecutionResults results, int bookingId)
        {
            createNotaFromBooking(results, bookingId, GeneralOperationsBookingReturnClass.All);
        }

        private static void createNotaFromBooking(BatchExecutionResults results, int bookingId, GeneralOperationsBookingReturnClass bookingType)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IGeneralOperationsBooking booking = GeneralOperationsBookingMapper.GetBooking(session, bookingId);
                if (booking != null)
                {
                    INota nota = booking.CreateNota();
                    if (nota != null)
                    {
                        NotaMapper.Update(session, nota);
                        results.MarkSuccess();
                    }
                    else
                        // in case NotaMigrated was set to 'true' (e.g. for a storno of a Monetary Order booking)
                        GeneralOperationsBookingMapper.Update(session, booking);
                }
                else
                    results.MarkError(new ApplicationException(string.Format("Booking {0} not found.", bookingId)));
            }
            catch (Exception ex)
            {
                results.MarkError(new ApplicationException(string.Format("Error creating nota for booking {0} ({1}).",
                                                                                       bookingId, bookingType), ex));
            }
            finally
            {
                session.Close();
            }
        }



        #endregion

        #region Printing Notas

        public static void PrintNotas(BatchExecutionResults results)
        {
            PrintNotas(results, 0);
        }

        public static void PrintNotas(BatchExecutionResults results, int managementCompanyId)
        {
            NotaReturnClass[] notaTypes = new NotaReturnClass[] {
                NotaReturnClass.NotaTransaction,
                NotaReturnClass.NotaDeposit,
                NotaReturnClass.NotaFees,
                NotaReturnClass.NotaDividend,
                NotaReturnClass.NotaTransfer,
                NotaReturnClass.NotaInstrumentConversion
            };

            foreach (NotaReturnClass notaType in notaTypes)
                NotaPrintCommand.PrintNotas(results, notaType, managementCompanyId);
        }

        public static void PrintNotas(BatchExecutionResults results, int managementCompanyId, NotaReturnClass notaType)
        {
            NotaPrintCommand.PrintNotas(results, notaType, managementCompanyId);
        }

        #endregion

        #region Sending Email Notifications

        private static DocumentSubtypes[] documentSubtypes = new DocumentSubtypes[] {
                                DocumentSubtypes.Notas,
                                DocumentSubtypes.QuarterlyReports
                            };

        public static void SendEmailNotifications(BatchExecutionResults results)
        {
            foreach (DocumentSubtypes documentSubtype in documentSubtypes)
                SendEmailNotifications(results, documentSubtype, 0);
        }

        public static void SendEmailNotifications(BatchExecutionResults results, int managementCompanyId)
        {
            foreach (DocumentSubtypes documentSubtype in documentSubtypes)
                SendEmailNotifications(results, documentSubtype, managementCompanyId);
        }

        public static void SendEmailNotifications(BatchExecutionResults results, DocumentSubtypes documentSubtype)
        {
            SendEmailNotifications(results, documentSubtype, 0, 0);
        }

        public static void SendEmailNotifications(BatchExecutionResults results, DocumentSubtypes documentSubtype, int managementCompanyId)
        {
            SendEmailNotifications(results, documentSubtype, managementCompanyId, 0);
        }

        public static void SendEmailNotifications(BatchExecutionResults results, DocumentSubtypes documentSubtype, int managementCompanyId, int accountId)
        {
            managementCompanyId = checkCompany(managementCompanyId);
            int[] freshAccountIds = getAccountIdsWithFreshDocuments(results, documentSubtype, managementCompanyId, accountId);

            foreach (int freshAccountId in freshAccountIds)
                sendEmailNotificationsForAccount(results, documentSubtype, freshAccountId);
        }

        private static int[] getAccountIdsWithFreshDocuments(BatchExecutionResults results, DocumentSubtypes documentSubtype, int managementCompanyId, int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                return DocumentMapper.GetAccountIdsWithFreshDocuments(session, documentSubtype, managementCompanyId, accountId);
            }
            catch (Exception ex)
            {
                results.MarkError(new ApplicationException(
                        string.Format("Error retrieving list of accounts needing email notifications ({0}).",
                                      Util.SplitCamelCase(documentSubtype.ToString())), ex));
                return new int[] { };
            }
            finally
            {
                session.Close();
            }
        }

        private static void sendEmailNotificationsForAccount(BatchExecutionResults results, DocumentSubtypes documentSubtype, int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                List<IDocument> documents = DocumentMapper.GetFreshDocuments(session, documentSubtype, accountId);
                if (documents.Count > 0)
                {
                    ICustomerAccount account = documents[0].Account;

                    if (account.Status == AccountStati.Active)
                    {
                        List<IContact> contacts = (from ah in account.AccountHolders
                                                   where ah.IsPrimaryAccountHolder || account.AccountHolders.Count == 1
                                                   select ah.Contact into c
                                                   where c.IsActive && c.Email != "" &&
                                                         c.ContactSendingOptions.GetValueOrDefault(
                                                                SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByEmail)
                                                   select c).ToList();

                        sendEmailNotificationsToContacts(results, contacts, documents, documentSubtype);
                    }

                    foreach (IDocument document in documents)
                        document.EmailNotificationHandled = true;
                    session.Update(documents);
                }
            }
            catch (Exception ex)
            {
                results.MarkError(new ApplicationException(
                    string.Format("Error sending email notifications for account {0}.", accountId), ex));
            }
            finally
            {
                session.Close();
            }
        }

        private static void sendEmailNotificationsToContacts(BatchExecutionResults results, List<IContact> contacts, List<IDocument> documents,
                                                             DocumentSubtypes documentSubtype)
        {
            if (contacts.Count > 0)
            {
                ICustomerAccount account = documents[0].Account;

                string body = documentsCreatedEmailTemplate;
                body = Utility.ShowOptionalTag(body, "option-notas", documentSubtype == DocumentSubtypes.Notas);
                body = Utility.ShowOptionalTag(body, "option-reports", documentSubtype == DocumentSubtypes.QuarterlyReports ||
                                                                       documentSubtype == DocumentSubtypes.YearlyReports);

                string clientWebsiteUrl = (account.AccountOwner.StichtingDetails.ClientWebsiteUrl ?? "").TrimEnd('/');
                if (string.IsNullOrEmpty(clientWebsiteUrl))
                    throw new ApplicationException("Client Website URL not known.");

                body = body.Replace("<%AccountNumber%>", account.Number)
                           .Replace("<%ClientWebsiteUrl%>", clientWebsiteUrl);

                MailMessage message = new MailMessage();
                message.Subject = string.Format("Nieuwe {0} portefeuille {1}",
                                                documentSubtype == DocumentSubtypes.Notas ? "transactienota’s" : "rapportage",
                                                account.Number);
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();

                foreach (IContact contact in contacts)
                {
                    message.To.Clear();
                    message.To.Add(testEmailRecipients != "" ? testEmailRecipients : contact.Email);
                    message.Body = body.Replace("<%DearSirForm%>", contact.CurrentNAW.Formatter.DearSirForm);

                    client.Send(message);
                    results.MarkSuccess();
                }
            }
        }

        private static int checkCompany(int managementCompanyId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IManagementCompany currentCompany = LoginMapper.GetCurrentManagmentCompany(session);
                if (currentCompany == null)
                    throw new ApplicationException("Could not find current management company.");

                if (currentCompany.IsStichting)
                {
                    if (managementCompanyId != 0)
                    {
                        IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
                        if (managementCompany == null)
                            throw new ApplicationException(string.Format("ManagementCompany {0} not found.", managementCompanyId));
                    }
                }
                else
                {
                    if (managementCompanyId == 0)
                        return currentCompany.Key;
                    if (managementCompanyId != currentCompany.Key)
                        throw new ApplicationException(string.Format("User not authorized for ManagementCompany {0}.", managementCompanyId));
                }

                return managementCompanyId;
            }
            finally
            {
                session.Close();
            }
        }

        #endregion

        #region Testing

        public static DataSet BuildTestDataSet(int accountId, NotaReturnClass notaType)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                return NotaPrintCommand.BuildTestDataSet(session, accountId, notaType);
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet BuildTestDataSet(int notaId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                return NotaPrintCommand.BuildTestDataSet(session, notaId);
            }
            finally
            {
                session.Close();
            }
        }

        #endregion

        #region Display Results

        public static string FormatErrorsForCreateNotas(BatchExecutionResults results, string currentManagmentCompanyName)
        {
            return formatErrors(results, "nota's", "created", "creating", "No new nota's need to be created", currentManagmentCompanyName);
        }

        public static string FormatErrorsForPrintNotas(BatchExecutionResults results, string currentManagmentCompanyName)
        {
            return formatErrors(results, "nota's", "printed", "printing", "No unprinted nota's were found", currentManagmentCompanyName);
        }

        public static string FormatErrorsForSendEmails(BatchExecutionResults results, string currentManagmentCompanyName)
        {
            return formatErrors(results, "email notifications", "sent", "sending", "No email notifications need to be sent", currentManagmentCompanyName);
        }

        private static string formatErrors(BatchExecutionResults results, string entities, string verbThirdForm, string verbContinuousForm, string noNeedMessage, 
                                           string currentManagmentCompanyName)
        {
            const int MAX_ERRORS_DISPLAYED = 25;

            string message = "<br/>";

            if (results.SuccessCount == 0 && results.ErrorCount == 0)
                message += string.Format("{0} for company '{1}'.", noNeedMessage, currentManagmentCompanyName);
            else
            {
                if (results.SuccessCount > 0)
                    message += string.Format("{0} {1} were successfully {2} for company '{3}'.<br/><br/><br/>",
                                             results.SuccessCount, entities, verbThirdForm, currentManagmentCompanyName);

                if (results.ErrorCount > 0)
                {
                    string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                        string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                    message += string.Format("{0} errors occured while {1} {2} for company '{3}'{4}:<br/><br/><br/>",
                                             results.ErrorCount, verbContinuousForm, entities, 
                                             currentManagmentCompanyName, tooManyErrorsMessage);

                    int errors = 0;
                    foreach (Exception ex in results.Errors)
                    {
                        if (++errors > MAX_ERRORS_DISPLAYED)
                            break;
                        message += Utility.GetCompleteExceptionMessage(ex) + "<br/>";
                    }
                }
            }

            return message;
        }

        #endregion

        private static string testEmailRecipients = ConfigurationManager.AppSettings.Get("TestEmailRecipients") != null ?
                                                        ConfigurationManager.AppSettings.Get("TestEmailRecipients").Trim() : "";
        private static string documentsCreatedEmailTemplate = Utility.ReadResource(Assembly.GetAssembly(typeof(PrintNotasAdapter)),
                                                                       "B4F.TotalGiro.ApplicationLayer.Reports.DocumentsCreatedEmail.htm");
    }
}
