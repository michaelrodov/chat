using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chatserver.Models
{
    public class SlimMessageModel
    {  
            public string TEXT { get; set; }
            public string FROM { get; set; }
            public string TO { get; set; }

            public static MESSAGE toMessage(SlimMessageModel slimMessage)
            {
                MESSAGE msg = new MESSAGE();
                msg.TEXT = slimMessage.TEXT;
                msg.TO = slimMessage.TO;
                msg.FROM = slimMessage.FROM;

                return msg;
            }
        }

    }
