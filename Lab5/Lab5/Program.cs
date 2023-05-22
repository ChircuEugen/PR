// In setarile proiectului(Lab5.csproj) adaugam referinta la Mail.dll pentru a folosi Limilabs
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            SendMessage("aiciSeIntroduceSenderul", "aiciReceiverul", "parolaSenderului", "Test", "Mesaj de testare");
            Console.ReadKey();
            
            // Afiseaza continutul mesajelor din INBOX
            ShowMessages("Senderul", "parola");
            Console.ReadKey();
        }

        public static void SendMessage(string from, string to, string password, string subject, string message)
        {
            // obiect de tipul MailMessage
            MailMessage mail = new MailMessage();

            // de la
            mail.From = new MailAddress(from);
            // catre
            mail.To.Add(to);
            // s.a.m.d
            mail.Subject = subject;
            mail.Body = "<h1>" + message + "</h1>";
            mail.IsBodyHtml = true;

            // avem nevoie de SMTP serverul al postei care o folosim ca Sender si portul acelui server
            // https://help.mail.ru/mail/mailer/popsmtp
            // Pe site scrie ca SMTP portul la MAIL.RU este 465, insa nu merge.
            // Cu portul de la OUTLOOK, 587, merge
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);

            // Autentificarea email-ului nostru la SMTP server
            smtp.Credentials = new NetworkCredential(from, password);
            smtp.EnableSsl = true;
            smtp.Send(mail);


            Console.WriteLine("Message was successfully sent to " + to);

        }

        public static void ShowMessages(string Email, string password)
        {
            Console.WriteLine("\nInboxMessages:");
            Imap imap = new Imap();
            // Avem nevoie de IMAP serverul al postei pe care o folosim
            // https://help.mail.ru/mail/mailer/popsmtp
            imap.Connect("imap.mail.ru");
            imap.UseBestLogin(Email, password);
            imap.SelectInbox();

            // uids va pastra ID-urile tuturor mesajelor din INBOX
            List<long> uids = imap.Search(Flag.New);
            foreach (long uid in uids)
            {
                // Preia continutul mesajului care se pastreaza dupa UID si le afiseaza
                IMail email = new MailBuilder().CreateFromEml(imap.GetMessageByUID(uid));
                if (!email.Subject.Contains(".dll"))
                    Console.WriteLine($"{email.Subject}");
            }
            imap.Close();
        }

    }
}
