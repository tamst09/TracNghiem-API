using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class MailBox
    {
        public int ID { get; set; }     
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public bool isRead { get; set; }
    }
}
