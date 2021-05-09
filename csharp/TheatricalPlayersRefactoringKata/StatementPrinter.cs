using System;
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
            var result = string.Format("Statement for {0}{1}", invoice.Customer, Environment.NewLine);
            CultureInfo cultureInfo = new CultureInfo("en-US");


            var computedPrice = ComputePrice(invoice, plays);
            foreach (var perf in invoice.Performances)
            {
                var price = computedPrice[perf.PlayID];
                var play = plays[perf.PlayID];
                totalAmount += price;
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats){3}", play.Name, Convert.ToDecimal(price / 100), perf.Audience, Environment.NewLine);   
            }

            volumeCredits = ComputeVolumeCredits(invoice, plays);

            result += String.Format(cultureInfo, "Amount owed is {0:C}{1}", Convert.ToDecimal(totalAmount / 100), Environment.NewLine);
            result += String.Format("You earned {0} credits{1}", volumeCredits, Environment.NewLine);
            
            return result;
        }

        private IDictionary<string, int> ComputePrice(Invoice invoice, IDictionary<string, Play> plays)
        {
            const int comedyPrice = 30000;

            const int comedyAudienceLimit = 20;

            const int comedyOverLimitTicketPrice = 500;
            const int comedyOverLimitFee = 10000;
            const int comedyTicketPrice = 300;

            IDictionary<string, int> playCosts = new Dictionary<string, int>();
            foreach (var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                switch (play.Type) 
                {
                    case "tragedy":
                        thisAmount += ComputeTragedyPrice(perf);
                        break;
                    case "comedy":
                        thisAmount = comedyPrice;
                        if (perf.Audience > comedyAudienceLimit) {
                            thisAmount += comedyOverLimitFee + comedyOverLimitTicketPrice * (perf.Audience - comedyAudienceLimit);
                        }
                        thisAmount += comedyTicketPrice * perf.Audience;
                        break;
                    default:
                        throw new Exception("unknown type: " + play.Type);
                }                
                playCosts[perf.PlayID] = thisAmount;
            }

            return playCosts;
        }

        private int ComputeTragedyPrice(Performance perf)
        {
            int finalPrice = 0;
            const int tragedyPrice = 40000;
            const int tragedyAudienceLimit = 30;
            const int tragedyOverLimitTicketPrice = 1000;

            finalPrice = tragedyPrice;
            if (perf.Audience > tragedyAudienceLimit)
            {
                finalPrice += tragedyOverLimitTicketPrice * (perf.Audience - tragedyAudienceLimit);
            }

            return finalPrice;
        }

        private int ComputeVolumeCredits(Invoice invoice, IDictionary<string, Play> plays)
        {
            int volumeCredits = 0;
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every five comedy attendees
                if ("comedy" == play.Type)
                    volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);
            }
            return volumeCredits;
        }
    }
}

