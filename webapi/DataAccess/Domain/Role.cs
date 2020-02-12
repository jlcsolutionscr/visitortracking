using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("role")]
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}