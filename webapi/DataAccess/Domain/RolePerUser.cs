using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("roleperuser")]
    public class RolePerUser
    {
        public int RoleId { get; set; }
        public string UserId { get; set; }
    }
}