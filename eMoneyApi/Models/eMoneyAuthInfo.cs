using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eMoneyApi.Models
{
    public class eMoneyAuthInfo
    {
        public string ClientAssertion { get; set; }
        public eMoneyAccessToken BearerToken { get; set; }
    }
}