using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers
{

    public static class OFXParser
    {

        private static string[] dateTimeFormats = new string[] { "yyyyMMddHHmmss[zz:EST]", "yyyyMMddHHmmss[z:GMT]" };

        public static OFXFile ReadFile(string filePath)
        {
            bool isHeader = true;
            
            OFXFile parsedFile = new OFXFile();
           
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line)==true)
                    {
                        continue;
                    }

                    if (line.ToUpper().Contains("<OFX>") == true)
                    {
                        isHeader = false;
                    }
                    else
                    {
                        string[] headerItem = line.Split(':');

                        parsedFile.Headers.Add(headerItem[0].ToUpper(), headerItem[1]);
                    }

                    if (isHeader == false)
                    {
                        string fileWithoutHeader = reader.ReadToEnd();
                        fileWithoutHeader = Regex.Replace(fileWithoutHeader, Environment.NewLine, "");
                        fileWithoutHeader = Regex.Replace(fileWithoutHeader, "\n", "");
                        fileWithoutHeader = Regex.Replace(fileWithoutHeader, "\t", "");

                        parsedFile.BankId = Regex.Match(fileWithoutHeader, @"(?<=BANKID>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                        parsedFile.AccountId = Regex.Match(fileWithoutHeader, @"(?<=ACCTID>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                        parsedFile.AccountType = Regex.Match(fileWithoutHeader, @"(?<=ACCTTYPE>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                        parsedFile.StartDate = Regex.Match(fileWithoutHeader, @"(?<=DTSTART>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                        parsedFile.EndDate = Regex.Match(fileWithoutHeader, @"(?<=DTEND>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();

                        string transactionList = Regex.Match(fileWithoutHeader, @"(?<=<BANKTRANLIST>).+(?=<\/BANKTRANLIST>)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();

                        MatchCollection m = Regex.Matches(transactionList, @"<STMTTRN>.+?<\/STMTTRN>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                       
                        foreach (Match match in m)
                        {
                            foreach (Capture capture in match.Captures)
                            {
                                OFXFileTransaction trans = new OFXFileTransaction();

                                trans.CheckNum = Regex.Match(capture.Value, @"(?<=<CHECKNUM>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                                trans.Type = Regex.Match(capture.Value, @"(?<=<TRNTYPE>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                                string dateString = Regex.Match(capture.Value, @"(?<=<DTPOSTED>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                                trans.DatePosted = DateTime.ParseExact(dateString, dateTimeFormats, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None);

                                trans.Amount = decimal.Parse(Regex.Match(capture.Value.Trim(), @"(?<=<TRNAMT>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim());
                                trans.FitId = Regex.Match(capture.Value, @"(?<=<FITID>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();
                                trans.Memo = Regex.Match(capture.Value, @"(?<=<MEMO>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.Trim();

                                parsedFile.Transactions.Add(trans);
                            }
                        }
                    }

                }


                return parsedFile;

            }
        }

    }
}
