using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("parameter")]
    public class Parameter
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }
}