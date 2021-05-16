using ADB2C.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADB2C.Model.Models
{
    public class User : ISoftDeleteableEntity
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public bool IsDeleted { get; set; }
    }
}
