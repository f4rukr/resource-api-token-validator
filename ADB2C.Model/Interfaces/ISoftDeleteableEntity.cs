using System;
using System.Collections.Generic;
using System.Text;

namespace ADB2C.Model.Interfaces
{
    public interface ISoftDeleteableEntity
    {
        public bool IsDeleted { get; set; }
    }
}
