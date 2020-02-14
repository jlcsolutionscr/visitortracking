using System.Collections.Generic;

namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class Session
    {
        public Session()
        {
            RolePerUser = new List<RoleItem>();
        }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyIdentifier { get; set; }
        public string Token { get; set; }
        public IList<RoleItem> RolePerUser { get; set; }
    }

    public class RoleItem
    {
        public RoleItem(int roleId, int userId)
        {
            RoleId = roleId;
            UserId = userId;
        }
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}