using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Identifier { get; set; }
        [NotMapped]
        public string Token { get; set; }
    }
}