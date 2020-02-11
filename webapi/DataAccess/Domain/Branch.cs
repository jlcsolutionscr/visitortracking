using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("branch")]
    public class Branch
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string AccessCode { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
    }
}