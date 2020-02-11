using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("registry")]
    public class Registry
    {
        public string DeviceId { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public DateTime RegisterDate { get; set; }
        public int VisitCount { get; set; }
        public string Status { get; set; }
    }
}