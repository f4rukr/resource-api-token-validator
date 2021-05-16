using System;
using System.Collections.Generic;
using System.Text;

namespace ADB2C.Model.Models.OIDC
{
    public class AuthErrorResponse
    {
        public string Version { get; set; }
        public int Status { get; set; }
        public string Code { get; set; }
        public string RequestId { get; set; }
        public string UserMessage { get; set; }
        public string DeveloperMessage { get; set; }
    }
}
