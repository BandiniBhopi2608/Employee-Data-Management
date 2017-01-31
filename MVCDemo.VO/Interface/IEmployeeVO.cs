using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCDemo.VO
{
    public interface IEmployeeVO
    {
        string Gender { get; set; }
        string City { get; set; }
        DateTime? DateOfBirth { get; set; }
    }
}
