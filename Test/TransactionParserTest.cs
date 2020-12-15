using System;
using System.IO;
using System.Linq;
using BankStatement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test3
{
    [TestClass]
    public class TransactionParserTest
    {

        [TestMethod]
        public void TestParser()
        {
            string filename = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),  @"sample2.txt");
            var stream = new StreamReader(filename); 

            AlbiladBankStatmentParser parser = new AlbiladBankStatmentParser();
            using (stream)
            {
                var transactions = parser.Parse(stream);
                var s = transactions.First();
                Assert.IsTrue(s.Balance == (decimal) 1000.00);
                Assert.IsTrue(s.Credit == 0);
                Assert.IsTrue(s.BeneficiaryId == "0000000000000");
                Assert.IsTrue(s.Beneficiary == "BIG STORE");
                Assert.IsTrue(s.Debt == (decimal)-10);
                Assert.IsTrue(s.Date == new DateTime(2000, 1, 1, 0, 00, 0));
                Assert.IsTrue(s.Type == "مشتريات نقطة بيع");
                Assert.IsTrue(s.ReferenceNumber == "FT0000000000000");
            }
        }

        [TestMethod]
        public void TestParserCount()
        {
            string location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            StreamReader reader = new StreamReader(Path.Combine(location, "sample2.txt"));
            AlbiladBankStatmentParser parser = new AlbiladBankStatmentParser();
            var transactions = parser.Parse(reader);
            Assert.IsTrue(transactions.Count() == 2);
        }
    }
}
