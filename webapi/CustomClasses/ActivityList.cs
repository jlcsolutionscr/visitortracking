using System;

namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class ActivityList
    {
        public ActivityList(int id, string name, string date, string type, string comment)
        {
            RegistryId = id;
            CustomerName = name;
            RegisterDate = date;
            ProductType = type;
            Comment = comment;
        }

        public int RegistryId { get; set; }
        public string CustomerName { get; set; }
        public string RegisterDate { get; set; }
        public string ProductType { get; set; }
        public string Comment { get; set; }
    }
}