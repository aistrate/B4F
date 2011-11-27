using System;
using System.Net.Mail;

namespace B4F.TotalGiro.Jobs.Manager.Notifier
{
	[Serializable]
	public class SmtpNotifier : AgentNotifier
	{
		public string From = "JobManager@TotalGiro.com";
		public string To;
		public string Subject = "[ALERT] {0}";
		public string Body = "{0}";
		public string SmtpServer = "localhost";

		protected override void Notify(WorkerResult result) 
		{
            MailAddress from = new MailAddress(From);
            MailAddress to = new MailAddress(To);
            MailMessage message = new MailMessage(from, to);
            message.Subject = string.Format(Subject, result.ShortMessage);
            message.Body = string.Format(Body, result.DetailedMessage);
            SmtpClient client = new SmtpClient(SmtpServer);
            client.Send(message);
        }
	}
}
