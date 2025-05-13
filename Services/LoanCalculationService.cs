using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWasmApp.Services
{
    public class LoanCalculationService
    {
        public class LoanResult
        {
            public decimal TotalPaid { get; set; }
            public decimal AdjustedForInflation { get; set; }
            public int Months { get; set; }
            public List<decimal> MonthlyPayments { get; set; } = new List<decimal>();
            public List<MonthlyBreakdown> MonthlyBreakdown { get; set; } = new List<MonthlyBreakdown>();
        }

        public class MonthlyBreakdown
        {
            public int MonthNumber { get; set; }
            public decimal Payment { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal RemainingBalance { get; set; }
            public decimal InflationAdjusted { get; set; }
        }

        private decimal CalculateInflationFactor(int month, decimal annualInflationRate)
        {
            // Convert annual inflation rate to monthly rate
            decimal monthlyInflationRate = annualInflationRate / 100 / 12;
            
            // Calculate the cumulative inflation factor for the given month
            // Using the formula: (1 + r)^n where r is the monthly rate and n is the number of months
            return (decimal)Math.Pow(1 + (double)monthlyInflationRate, month);
        }

        public LoanResult CalculateMinimumPayment(decimal loanAmount, decimal annualInterestRate, int loanTermYears, decimal annualInflationRate, decimal extraPayment = 0)
        {
            var monthlyInterestRate = annualInterestRate / 100 / 12;
            var monthlyInflationRate = annualInflationRate / 100 / 12;
            var numberOfPayments = loanTermYears * 12;

            // Calculate minimum monthly payment using the loan amortization formula
            var minimumPayment = loanAmount * (monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, numberOfPayments)) /
                                ((decimal)Math.Pow(1 + (double)monthlyInterestRate, numberOfPayments) - 1);

            // Add extra payment to the minimum payment
            var totalMonthlyPayment = minimumPayment + extraPayment;

            var remainingBalance = loanAmount;
            var totalPaid = 0m;
            var monthlyPayments = new List<decimal>();
            var monthlyBreakdown = new List<MonthlyBreakdown>();

            // Calculate monthly breakdown
            for (int month = 1; month <= numberOfPayments; month++)
            {
                var interestPayment = remainingBalance * monthlyInterestRate;
                var principalPayment = totalMonthlyPayment - interestPayment;

                // If this would pay off the loan, adjust the payment
                if (principalPayment > remainingBalance)
                {
                    principalPayment = remainingBalance;
                    totalMonthlyPayment = principalPayment + interestPayment;
                }

                remainingBalance -= principalPayment;
                totalPaid += totalMonthlyPayment;

                // Calculate inflation-adjusted payment
                var inflationFactor = CalculateInflationFactor(month, annualInflationRate);
                var inflationAdjusted = totalMonthlyPayment / inflationFactor;

                monthlyPayments.Add(totalMonthlyPayment);
                monthlyBreakdown.Add(new MonthlyBreakdown
                {
                    MonthNumber = month,
                    Payment = totalMonthlyPayment,
                    Principal = principalPayment,
                    Interest = interestPayment,
                    RemainingBalance = remainingBalance,
                    InflationAdjusted = inflationAdjusted
                });

                // If loan is paid off, break the loop
                if (remainingBalance <= 0)
                {
                    break;
                }
            }

            // Calculate total inflation-adjusted amount
            var totalAdjustedForInflation = monthlyBreakdown.Sum(m => m.InflationAdjusted);

            return new LoanResult
            {
                TotalPaid = totalPaid,
                AdjustedForInflation = totalAdjustedForInflation,
                Months = monthlyBreakdown.Count,
                MonthlyPayments = monthlyPayments,
                MonthlyBreakdown = monthlyBreakdown
            };
        }

        public LoanResult CalculateAggressivePayment(decimal loanAmount, decimal annualInterestRate, int loanTermYears, decimal extraPayment, decimal inflationRate = 0)
        {
            decimal monthlyInterestRate = annualInterestRate / 100 / 12;
            int maxMonths = loanTermYears * 12;
            
            // Calculate minimum monthly payment
            decimal minimumMonthlyPayment = loanAmount * (monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, maxMonths)) 
                                          / ((decimal)Math.Pow(1 + (double)monthlyInterestRate, maxMonths) - 1);
            
            decimal monthlyPayment = minimumMonthlyPayment + extraPayment;
            decimal remainingBalance = loanAmount;
            decimal totalPaid = 0;
            var monthlyPayments = new List<decimal>();
            var monthlyBreakdown = new List<MonthlyBreakdown>();
            int months = 0;

            while (remainingBalance > 0 && months < maxMonths)
            {
                months++;
                decimal interestPayment = remainingBalance * monthlyInterestRate;
                decimal principalPayment = monthlyPayment - interestPayment;
                
                if (principalPayment > remainingBalance)
                {
                    principalPayment = remainingBalance;
                    monthlyPayment = principalPayment + interestPayment;
                }

                remainingBalance -= principalPayment;
                totalPaid += monthlyPayment;
                monthlyPayments.Add(monthlyPayment);

                // Calculate inflation-adjusted payment
                decimal inflationFactor = CalculateInflationFactor(months, inflationRate);
                decimal inflationAdjustedPayment = monthlyPayment / inflationFactor;

                // Add monthly breakdown
                monthlyBreakdown.Add(new MonthlyBreakdown
                {
                    MonthNumber = months,
                    Payment = monthlyPayment,
                    Principal = principalPayment,
                    Interest = interestPayment,
                    RemainingBalance = remainingBalance,
                    InflationAdjusted = inflationAdjustedPayment
                });
            }

            // Calculate total inflation-adjusted amount
            decimal totalInflationAdjusted = monthlyBreakdown.Sum(m => m.InflationAdjusted);

            return new LoanResult
            {
                TotalPaid = totalPaid,
                AdjustedForInflation = totalInflationAdjusted,
                Months = months,
                MonthlyPayments = monthlyPayments,
                MonthlyBreakdown = monthlyBreakdown
            };
        }
    }
} 