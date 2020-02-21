using System;

namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class RegistryData
    {
        public RegistryData(int id, string name, string date)
        {
            RegistryId = id;
            CustomerName = name;
            RegisterDate = date;
        }

        public int RegistryId { get; set; }
        public string CustomerName { get; set; }
        public string RegisterDate { get; set; }
    }
}