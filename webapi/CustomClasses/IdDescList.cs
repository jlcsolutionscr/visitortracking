namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class IdDescList
    {
        public IdDescList(int id, string desc)
        {
            Id = id;
            Description = desc;
        }
        public int Id { get; set; }
        public string Description { get; set; }
    }
}