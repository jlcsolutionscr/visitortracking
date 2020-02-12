using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("authorizationentry")]
    public class AuthorizationEntry
    {
        [Key]
        public string Id { get; set; }
        public DateTime EmitedAt { get; set; }
    }
}