using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eMoneyApi.Models
{
    public class eMoneyAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
    }
}