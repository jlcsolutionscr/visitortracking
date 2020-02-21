using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("roleperuser")]
    public class RolePerUser
    {
        public int RoleId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}