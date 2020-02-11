namespace jlcsolutionscr.com.visitortracking.webapi.customclasses
{
    public class AppSettings
    {
        public bool IsProduction { get; set; }
        public string ConnectionString { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SSLHost { get; set; }
        public string MailUser { get; set; }
        public string MailPassword { get; set; }
    }
}