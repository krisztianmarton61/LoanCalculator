using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWasmApp.Services
{
    public class LoanCalculationService
    {
        public class LoanResult
        {
            public decimal MonthlyPayment { get; set; }
            public decimal TotalInterest { get; set; }
            public decimal TotalPayment { get; set; }
            public List<MonthlyPayment> MonthlyPayments { get; set; } = new List<MonthlyPayment>();
        }

        public class MonthlyPayment
        {
            public int Month { get; set; }
            public decimal Payment { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal RemainingBalance { get; set; }
        }

        public LoanResult CalculateLoan(decimal loanAmount, decimal interestRate, int loanTerm, decimal additionalPayment)
        {
            decimal monthlyRate = interestRate / 100 / 12;
            int numberOfPayments = loanTerm * 12;
            decimal remainingBalance = loanAmount;
            var monthlyPayments = new List<MonthlyPayment>();

            decimal monthlyPayment = (loanAmount * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, numberOfPayments)) /
                                   ((decimal)Math.Pow(1 + (double)monthlyRate, numberOfPayments) - 1);

            decimal totalPayment = 0;
            decimal totalInterest = 0;

            for (int month = 1; month <= numberOfPayments; month++)
            {
                decimal interestPayment = remainingBalance * monthlyRate;
                decimal principalPayment = monthlyPayment - interestPayment + additionalPayment;
                remainingBalance -= principalPayment;

                if (remainingBalance < 0)
                {
                    principalPayment += remainingBalance;
                    remainingBalance = 0;
                }

                monthlyPayments.Add(new MonthlyPayment
                {
                    Month = month,
                    Payment = monthlyPayment + additionalPayment,
                    Principal = principalPayment,
                    Interest = interestPayment,
                    RemainingBalance = remainingBalance
                });

                totalPayment += monthlyPayment + additionalPayment;
                totalInterest += interestPayment;

                if (remainingBalance <= 0)
                    break;
            }

            return new LoanResult
            {
                MonthlyPayment = monthlyPayment,
                TotalInterest = totalInterest,
                TotalPayment = totalPayment,
                MonthlyPayments = monthlyPayments
            };
        }
    }
} 