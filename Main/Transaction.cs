using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BankStatement{
    public class Transaction 
    {
        [Format("{0:c}")]
        public decimal Balance { get; set; }
        
        [Format("c")]
        public decimal Credit { get; set; }
        
        [Display(Name = "مدين")]
        [Format("c")]
        public decimal Debt { get; set; }

        public string Type { get; set; }

        [Display(Name = "المدينة")]
        public string City { get; set; }

        public string Beneficiary { get; set; }
        public string BeneficiaryId { get; set; }
        public string ReferenceNumber { get; set; }
        public string From { get; set; }
        public string Location { get; set; }

        [Format("G")]
        public System.DateTime Date { get; set; }

    }
}