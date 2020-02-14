using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("employee")]
    public class Employee
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
    }
}