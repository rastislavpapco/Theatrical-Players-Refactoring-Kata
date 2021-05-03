﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            const int tragedyPrice = 40000;
            const int comedyPrice = 30000;

            const int tragedyAudienceLimit = 30;
            const int comedyAudienceLimit = 20;

            foreach (var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                switch (play.Type) 
                {
                    case "tragedy":
                        thisAmount = tragedyPrice;
                        if (perf.Audience > tragedyAudienceLimit) {
                            thisAmount += 1000 * (perf.Audience - tragedyAudienceLimit);
                        }
                        break;
                    case "comedy":
                        thisAmount = comedyPrice;
                        if (perf.Audience > comedyAudienceLimit) {
                            thisAmount += 10000 + 500 * (perf.Audience - comedyAudienceLimit);
                        }
                        thisAmount += 300 * perf.Audience;
                        break;
                    default:
                        throw new Exception("unknown type: " + play.Type);
                }
                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) 
                    volumeCredits += (int)Math.Floor((decimal)perf.Audience / 10);

                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                totalAmount += thisAmount;
            }

            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += String.Format("You earned {0} credits\n", volumeCredits);

            return result;
        }

    }
}

