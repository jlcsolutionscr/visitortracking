using System;

namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class ActivityResume
    {
        public ActivityResume(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name { get; set; }
        public int Count { get; set; }
    }
}