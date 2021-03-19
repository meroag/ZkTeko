using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using DataUpdater.Classes;

namespace DataUpdater
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("IP:");
            var ip = Console.ReadLine();
            Console.Write("Port:");
            var port = Console.ReadLine();
            UpdateFingerprintData(ip, Convert.ToInt32(port));
        }

        private static void UpdateFingerprintData(string ip,int port)
        {
            FingerPrintDevice device = new FingerPrintDevice(false);
            try
            {
                device.Connect(ip, port);
                Console.WriteLine($"Connecting to {ip}:{port}");
                var data = device.GetDataFromDevice();
                Console.WriteLine($"Got {data.Count} records from this device");
                foreach (FingerPrintRecord record in data)
                {
                    Console.WriteLine($"{record.Date}/{record.IdUser}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine($"Disconneting from {ip}");
                device.Discoonect();
            }

            Console.ReadKey();
        }

        private static void SendMyData()
        {
            Console.WriteLine("Sending mail to my :)");
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;
            var dt = SQL.Select("SELECT Name,LastSyncTime FROM dbo.Devices WHERE ActiveStatus = 1");
            MailMessage msg = new MailMessage();
            msg.Subject = "გაცდენების მონიტროინგის აპლიკაციის ბოლო განახლება";
            msg.To.Add("m_agniashvili@trd.ge");
            var txt = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                txt.AppendLine(row["Name"] + ":" + row["LastSyncTime"]);
            }

            msg.Body = txt.ToString();
            SendExchange(msg, false);
        }

        public static void SendData()
        {
            int daysToSubstract = -1;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                daysToSubstract = -2;
            var dtDepartaments =
                SQL.Select("SELECT ID,Name,Email FROM dbo.Departments WHERE Active = 1 AND Email != ''");
            var dtTodaysCheckIns = SQL.Select("EXECUTE dbo.spGetCheckins " +
                                              "@Date1 = '" +
                                              DateTime.Now.AddDays(daysToSubstract).Date.ToShortDateString() + "', " +
                                              "@Date2 = '" +
                                              DateTime.Now.AddDays(daysToSubstract).Date.ToShortDateString() + "'");
            Console.WriteLine("Sending data to head of departments");
            foreach (DataRow row in dtDepartaments.Rows)
            {
                MailMessage msg = new MailMessage();
                msg.Subject = row["Name"] + " დეპარტამენტის გაცდენების ინფორმაცია";

                var emails = row["Email"].ToString();
                if (emails.Split(';').Length > 1)
                {
                    var spletedAddress = emails.Split(';');
                    for (int i = 0; i < spletedAddress.Length; i++)
                    {
                        msg.To.Add(spletedAddress[i]);
                    }
                }
                else
                {
                    msg.To.Add(emails);
                }

                StringBuilder mailText = new StringBuilder();
                var checkIns = dtTodaysCheckIns.Select("IdDepartment = " + row["ID"]);
                foreach (DataRow checkInRow in checkIns)
                {
                    var _in = checkInRow["CheckIn"].ToString();
                    var _out = checkInRow["CheckOut"].ToString();
                    var comment = checkInRow["Comment"].ToString();
                    var person = checkInRow["Person"].ToString();

                    if (!string.IsNullOrEmpty(comment)) continue;

                    if (string.IsNullOrEmpty(_in) && string.IsNullOrEmpty(_out))
                    {
                        mailText.AppendLine(
                            person + "-ს არ აქვს განსაზღვრული არც შემოსვლა არც გასვლა გთხოვთ მიუთითოთ კომენტარი");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_in))
                            mailText.AppendLine(person + "-ს არ აქვს განსაზღვრული შემოსვლა გთხოვთ მიუთითოთ კომენტარი");
                        if (string.IsNullOrEmpty(_out))
                            mailText.AppendLine(person + "-ს არ აქვს განსაზღვრული გასვლა გთხოვთ მიუთითოთ კომენტარი");
                    }
                }

                if (string.IsNullOrEmpty(mailText.ToString()))
                    continue;
                //msg.CC.Add("m_agniashvili@trd.ge");
                msg.Body = mailText.ToString();
                msg.From = new MailAddress("m_agniashvili@trd.ge",
                    "T&R დისტრიბუშენის თანამშრომლების აღრიცხვის სისტემა");
                SendExchange(msg);
            }
        }

        private static void SendExchange(MailMessage msg, bool delay = true)
        {
            msg.From = new MailAddress("info@trd.ge", "T&R Distributuion fingerprint monitoring system");

            SmtpClient client = new SmtpClient();
            client.Host = "smtp.office365.com";
            client.Port = 587;
            client.EnableSsl = true;

            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("info@trd.ge", "trd@123456");
            client.Send(msg);
            Console.WriteLine($"Mail sent done");
            if (delay)
                Thread.Sleep(30 * 1000);

        }
    }
}
