using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("registry")]
    public class Registry
    {
        [Key]
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public DateTime RegisterDate { get; set; }
        public int VisitCount { get; set; }
        public string Status { get; set; }

        public Customer Customer { get; set; }
    }
}