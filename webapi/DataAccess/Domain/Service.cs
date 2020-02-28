using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("service")]
    public class Service
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}