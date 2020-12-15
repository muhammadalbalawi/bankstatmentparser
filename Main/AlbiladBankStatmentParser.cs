using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BankStatement 
{
    public class AlbiladBankStatmentParser 
    {
        public AlbiladBankStatmentParser() {
        }
        

        public IEnumerable<Transaction> Parse(StreamReader reader)
        {
            string line = reader.ReadLine().Trim();
            StringBuilder transactionDetails = new StringBuilder();
            while (!reader.EndOfStream)
            {
                if (line.StartsWith("Close Balance"))
                    break;
                Regex transactionRgx = new Regex(@"([\d.]+)\s+([\d.]+)\s+([\d.-]+)\s*([\w ]+)(\d{2}\/\d{2}\/\d{4})", RegexOptions.IgnoreCase);
                Match f = transactionRgx.Match(line);
                if (f.Success)
                {
                    Transaction tr = new Transaction
                    {
                        Balance = Decimal.Parse(f.Groups[1].Value),
                        Credit = decimal.Parse(f.Groups[2].Value),
                        Debt = decimal.Parse(f.Groups[3].Value),
                        Type = f.Groups[4].Value.Trim(),
                        Date =  DateTime.ParseExact(f.Groups[5].Value, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };

                    // read transaction deails in the following lines before next transaction
                    line = reader.ReadLine();
                    while (!transactionRgx.IsMatch(line) && !line.StartsWith("Close Balance")  && !reader.EndOfStream)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            transactionDetails.Append(line.TrimStart() + " ");
                        }
                        line = reader.ReadLine();
                    }

                    string details = transactionDetails.ToString();
                    if (tr.Type == "POS Purchase")
                    {
                        PointOfSale(details, ref tr);
                    }
                    else if (tr.Type == "SADAD Payment")
                    {
                        SadadPayment(details, ref tr);
                    }
                    else if (tr.Type == "Cash Withdrawl")
                    {
                        CashWithdrawl(details, ref tr);
                    }
                    else if (tr.Type == "Settlement of Credit Card")
                    {
                        SettlementOfCreditCard(details, ref tr);
                    }
                    else if (tr.Type == "SARIE incoming transfer")
                    {
                        SarieIncomingTransfer(details, ref tr);
                    }
                    else if (tr.Type == "Sarie Outgoing Transfer")
                    {
                        SarieOutgoingTransfer(details, ref tr);
                    }
                    else if (tr.Type == "Payroll incoming transfer")
                    {

                    }
                    
                    transactionDetails.Clear();
                    yield return tr;
                }
            }
        }

        private void SettlementOfCreditCard(string details, ref Transaction tr)
        {
            tr.Type = "سداد بطاقة إئتمانية";
        }

        private void SarieOutgoingTransfer(string details, ref Transaction tr)
        {
            tr.Type = "حوالة خارجة";
            Match match = Regex.Match(details, @"To ([\W ]+) ‬In", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                
            }
        }

        private void SarieIncomingTransfer(string details, ref Transaction tr)
        {
            tr.Type = "حوالة قادمة";
            Match match = Regex.Match(details, @"from ([\W ]+). On", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                tr.From = match.Groups[1].Value;
            }
            var date = ParseDate(details);
            if (date.HasValue)
                tr.Date = date.Value;
        }

        private void CashWithdrawl(string details, ref Transaction tr)
        {
            tr.Type = "سحب نقدي";
            Match match = Regex.Match(details, @"via ([\W ]+) + ([\W ]+) On\s*,Ref\s*No\.\s*(FT\d+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {               
                tr.City = match.Groups[1].Value;
                tr.Location = match.Groups[2].Value;
                tr.ReferenceNumber = match.Groups[3].Value;
            }
            var date = ParseDate(details);
            if (date.HasValue)
                tr.Date = date.Value;
        }

        private void SadadPayment(string details, ref Transaction tr )
        {
            tr.Type = "مدفوعات سداد";
            Match match = Regex.Match(details, @"\s*,Ref\s*No\.\s*(FT\d+)[\w ]*On (\d{4}-\d{2}-\d{2} & \d{2}:\d{2})", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                tr.ReferenceNumber = match.Groups[1].Value;
                tr.Date = ParseDate2(match.Groups[2].Value);
            }
        }

        public void PointOfSale(string details, ref Transaction tr)
        {
            tr.Type = "مشتريات نقطة بيع";
            Match match = Regex.Match(details, @"from POS No\. ([\d-]+) In\s+([\w ]+) In (\w+) , ([\w ]+)\s+On (\d{4}-\d{2}-\d{2} & \d{2}:\d{2})\s*,Ref\s*No\.\s*(FT\d+)");
            if (match.Success)
            {
                tr.BeneficiaryId = match.Groups[1].Value;
                tr.Beneficiary = match.Groups[2].Value.Replace("\r\n", "");
                tr.City = match.Groups[3].Value;
                tr.Location = match.Groups[4].Value;
                tr.Date = ParseDate2(match.Groups[5].Value);
                tr.ReferenceNumber = match.Groups[6].Value;
            }
        }

        private DateTime? ParseDate(string details)
        {
            Match dateMatch = Regex.Match(details, @"\s+On (\d{4}-\d{2}-\d{2} & \d{2}:\d{2})");
            if (dateMatch.Success) 
                return DateTime.ParseExact(dateMatch.Groups[1].Value, "yyyy-MM-dd & HH:mm", CultureInfo.InvariantCulture);
            return null;
        }

        private DateTime ParseDate2(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd & HH:mm", CultureInfo.InvariantCulture);
        }
    }
}