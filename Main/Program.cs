using BankStatement;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing Arguments.");
                Console.WriteLine(@"extract transactions from Alibald bank statment pdf file into csv format.
                Usage: program.exe  password  pdffile  csvfile \n\n
                password : the password for encrypted pdf file \n
                pdffile  : the input pdf bankstatment file \n
                csvfile : the output csv file.");
            }

            string password = args[0];
            string input = args[1];
            string output =  args[2];

            Process process = new Process();
            process.StartInfo.FileName = "pdftotext.exe";
            process.StartInfo.Arguments = $" -opw {password} -table -marginl 50 -margint 260 -marginb 80 -enc UTF-8 {input} -";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            var stream = process.StandardOutput;
            AlbiladBankStatmentParser bankStatmentParser = new AlbiladBankStatmentParser();
            var transactions = bankStatmentParser.Parse(stream);
            using (var writer = new StreamWriter(output))
            {
               using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
               {
                   csv.Configuration.RegisterClassMap<TransactionMap>();
                   csv.WriteRecords(transactions);
                }
            }
        }
    }


    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Map(m => m.Balance).Index(0).Name("الرصيد");
            Map(m => m.Date).Index(1).Name("التاريخ");
            Map(m => m.Credit).Index(2).Name("دائن");
            Map(m => m.Debt).Index(3).Name("مدين");
            Map(m => m.Type).Index(4).Name("نوع العملية");
            Map(m => m.Beneficiary).Index(5).Name("المستفيد");
            Map(m => m.City).Index(6).Name("المدينة");
            Map(m => m.BeneficiaryId).Index(7).Name("معرف المستفيد");
            Map(m => m.ReferenceNumber).Index(8).Name("معرف العملية");
        }
    }

}
