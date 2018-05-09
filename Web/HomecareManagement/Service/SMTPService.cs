using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace HomecareManagement.Service
{
    public class SMTPService
    {
        public static class Account //這個物件用來設定 SMTP 位置、帳號、密碼
        {
            public const string EmailSMTPServer = "smtp.gmail.com"; //SMTP位置
            public const int EmailSMTPPort = 587;                   //SMTP Port

            public const string EmailSMTPName = "u101b117";  //發信者名稱
            public const string EmailSMTPUser = "u101b117@hk.edu.tw";  //SMTP 帳號 (ex:00xx@gmail.com)
            public const string EmailSMTPPass = "demonic0712";  //SMTP 密碼
        }

        public class MailSender
        {
            public string Subject { get; set; }             //信件標題
            public string Content { get; set; }             //信件內容
            public MailAddress ReplayTo { get; set; }       //信件回覆位置
            public List<MailAddress> Receiver { get; set; } //信件接收者
            public List<Attachment> Attachment { get; set; }//附件
            public List<MailAddress> CC { get; set; }       //副本
            public List<MailAddress> BCC { get; set; }      //密件副本
            public bool EnableSSL { get; set; }             //是否啟用SSL
            public bool EnableTHML { get; set; }            //是否為HTML內容


            public MailSender()
            {
                Receiver = new List<MailAddress>();
                CC = new List<MailAddress>();
                BCC = new List<MailAddress>();
                this.EnableSSL = true;
                this.EnableTHML = true;
            }

            /// <summary>
            /// 信件發送函數。
            /// </summary>
            /// <param name="Subject">信件標題</param>
            /// <param name="Content">信件內容</param>
            /// <param name="Receiver">收件者</param>
            public void Send(string Subject, string Content, List<MailAddress> Receiver, List<Attachment> Attachment)
            {
                this.Subject = Subject;
                this.Content = Content;
                this.Receiver = Receiver;
                this.Attachment = Attachment;
                Send();
            }
            /// <summary>
            /// 信件發送函數
            /// </summary>
            public void Send()
            {
                MailAddress mailFrom = new MailAddress(Account.EmailSMTPUser, Account.EmailSMTPName, Encoding.UTF8);
                MailMessage mail = new MailMessage();
                mail.From = mailFrom;                   //設定發件者 (gmail一定要跟登入帳戶一致)
                foreach (var item in Receiver)
                {
                    mail.To.Add(item);                  //加入信件接收者
                }
                foreach (var item in Attachment)
                {
                    mail.Attachments.Add(item);                  //加入信件接收者
                }
                mail.IsBodyHtml = EnableTHML;           //信件本文是否為HTML
                mail.Body = Content;                    //設定本文內容
                mail.BodyEncoding = Encoding.UTF8;
                mail.Subject = Subject;                 //設定信件標題
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Priority = MailPriority.High;
                mail.ReplyToList.Add(mailFrom);                //設定信件回覆位置
                foreach (var item in CC)
                {
                    mail.CC.Add(item);                  //加入信件副本
                }
                foreach (var item in BCC)
                {
                    mail.Bcc.Add(item);                 //加入信件密件副本
                }

                SmtpClient SC = new SmtpClient(Account.EmailSMTPServer, Account.EmailSMTPPort);
                SC.Credentials = new NetworkCredential(Account.EmailSMTPUser, Account.EmailSMTPPass);
                SC.EnableSsl = EnableSSL;   //是否開啟SSL
                SC.Send(mail);              //發送
            }
        }
    }
}