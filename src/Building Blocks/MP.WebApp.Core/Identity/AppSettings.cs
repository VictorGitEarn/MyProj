using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.WebApp.Core.Identity
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int ExpirationHours { get; set; }
        public string Issuer { get; set; }
        public string ValidIn { get; set; }

    }
}
