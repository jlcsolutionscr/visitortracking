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
        [ForeignKey("Registry")]
        public int RegistryId { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int EmployeeId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public DateTime VisitDate { get; set; }
        public bool Applied { get; set; }

        public Registry Registry { get; set; }
    }
}