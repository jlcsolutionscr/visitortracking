using System;

namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class ActivityList
    {
        public ActivityList(int id, string name, string date, string type)
        {
            RegistryId = id;
            CustomerName = name;
            RegisterDate = date;
            ProductType = type;
        }

        public int RegistryId { get; set; }
        public string CustomerName { get; set; }
        public string RegisterDate { get; set; }
        public string ProductType { get; set; }
    }
}