using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using jlcsolutionscr.com.visitortracking.webapi.customclasses;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("user")]
    public class User
    {
        public User () {
            RoleList = new List<IdDescList>();
        }

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Identifier { get; set; }
        [NotMapped]
        public List<IdDescList> RoleList { get; set; }
    }
}