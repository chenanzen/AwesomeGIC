using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AwesomeGIC
{
    public class PrintStatementProcessor : IProcessor
    {
        private readonly IIOService _ioService;
        private readonly IGICDataAccess _gicDataAccess;

        public PrintStatementProcessor(IIOService ioService, IGICDataAccess gicDataAccess)
        {
            _ioService = ioService;
            _gicDataAccess = gicDataAccess;
        }

        public void Process()
        {
            bool keepPrompting = true;
            while (keepPrompting)
            {
                var input = _ioService.GetInput(GICConstants.TransactionMenu);

                if (string.IsNullOrEmpty(input)) keepPrompting = false;
                else
                {
                    var inputToken = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (inputToken.Length == 2)
                    {
                        var accountName = inputToken[0];
                        var dateString = inputToken[1];

                        var isValidDate = DateTime.TryParseExact(dateString, GICConstants.InputYearMonthFormat,
                            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                            out DateTime inputDate);

                        if (isValidDate)
                        {
                            keepPrompting = false;

                            // get transactions in selected month
                            var acc = _gicDataAccess.GetAccount(accountName);
                            var accOfSelectedMonth = acc.Transactions
                                .Where(t => t.TransactionDateTime.Year == inputDate.Year && t.TransactionDateTime.Month == inputDate.Month)
                                .ToList();
                            // add start of month balance
                            var startOfMonthBalance = acc.Transactions
                                .Where(t => t.TransactionDateTime < inputDate)
                                .Sum(t => transactionValue(t));
                            accOfSelectedMonth.Add(new GICTransaction("temp", TransactionType.D, inputDate, startOfMonthBalance));
                            // add end of month marker to assist in calculation
                            var upperLimit = inputDate.AddMonths(1);
                            accOfSelectedMonth.Add(new GICTransaction("temp", TransactionType.D, upperLimit, 0));

                            // get interest settings in seleted month
                            var allInterestSettings = _gicDataAccess.GetInterestSetting().Select(i => i.Value)
                                .ToList();
                            var interestSettings = allInterestSettings
                                .Where(t => t.InterestSettingDateTime.Year == inputDate.Year && t.InterestSettingDateTime.Month == inputDate.Month)
                                .ToList();
                            // add start of month interest
                            var startOfMonthInterest = interestSettings
                                .Where(t => t.InterestSettingDateTime == inputDate).FirstOrDefault();
                            if (startOfMonthInterest == null)
                            {
                                // there is no interest value assigned for start of month. get value from interest set earlier
                                startOfMonthInterest = allInterestSettings
                                    .Where(t => t.InterestSettingDateTime < inputDate).FirstOrDefault();
                                if (startOfMonthInterest == null)
                                    interestSettings.Add(new GICInterestSetting(inputDate, "default", GICConstants.DefaultInterest));
                                else
                                    interestSettings.Add(new GICInterestSetting(inputDate, "default", startOfMonthInterest.InterestSettingValue));
                            }

                            // get date check point to assist in calculation
                            var importantDates = accOfSelectedMonth.Select(t => t.TransactionDateTime).ToList();
                            importantDates = importantDates.Union(interestSettings.Select(it => it.InterestSettingDateTime)).ToList();
                            importantDates = importantDates.Distinct().OrderBy(d => d).ToList();

                            var annualizedInterest = 0m;
                            var balance = 0m;
                            var interest = 0m;
                            var numOfDays = 0;
                            for(int i = 0; i < importantDates.Count -1; i++)
                            {
                                // get calculation from i to i+1
                                balance = accOfSelectedMonth
                                    .Where(acc => acc.TransactionDateTime < importantDates[i + 1])
                                    .Select(acc => transactionValue(acc)).Sum();
                                interest = interestSettings.Where(it => it.InterestSettingDateTime < importantDates[i + 1]).First().InterestSettingValue;
                                numOfDays = (importantDates[i+1] - importantDates[i]).Days;
                                annualizedInterest += (balance * interest * numOfDays) / 100;
                            }
                            var totalInterest = Math.Round((annualizedInterest / 365), 2);
                            accOfSelectedMonth.RemoveAll(t => t.TransactionId == "temp");
                            accOfSelectedMonth.Add(new GICTransaction(string.Empty, TransactionType.I, upperLimit.AddDays(-1), totalInterest));

                            var statementBalance = startOfMonthBalance;
                            var statement = new List<GICStatement>();
                            foreach(var transaction in accOfSelectedMonth)
                            {
                                statementBalance += transactionValue(transaction);
                                statement.Add(new GICStatement(transaction.TransactionId, transaction.TransactionType, transaction.TransactionDateTime, transaction.Amount, statementBalance));

                            }

                            _ioService.PrintStatement(acc.AccountName, statement);
                        }
                    }
                }
            }
        }

        private decimal transactionValue(GICTransaction transaction)
        {
            var modifier = 1;
            if (transaction.TransactionType == TransactionType.W) modifier = -1;

            var value = transaction.Amount * modifier;

            return value;
        }
    }
}
