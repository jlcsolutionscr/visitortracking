using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("activity")]
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public DateTime VisitDate { get; set; }
        public bool Applied { get; set; }
    }
}