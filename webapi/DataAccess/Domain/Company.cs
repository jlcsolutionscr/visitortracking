using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain
{
    [Table("company")]
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public byte[] Logotype{ get; set; }
        public string Identifier { get; set; }
        public string CompanyAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int PromotionAt { get; set; }
        public string PromotionDescription { get; set; }
        public string PromotionMessage { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UtcTimeFactor { get; set; }
    }
}