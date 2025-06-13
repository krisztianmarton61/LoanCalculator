let paymentChart = null;

window.createPaymentChart = function (payments, interestPayments, loanAmount) {
    const canvas = document.getElementById('paymentChart');
    if (!canvas) {
        console.error('Canvas element not found');
        return;
    }

    const ctx = canvas.getContext('2d');
    if (!ctx) {
        console.error('Could not get 2D context');
        return;
    }
    
    if (paymentChart) {
        paymentChart.destroy();
    }

    const labels = Array.from({ length: payments.length }, (_, i) => `Month ${i + 1}`);
    
    const loanPlusInterest = interestPayments.reduce((acc, curr, i) => {
        acc.push(i === 0 ? loanAmount + curr : acc[i - 1] + curr);
        return acc;
    }, []);

    const cumulativePayments = payments.reduce((acc, curr, i) => {
        acc.push(i === 0 ? curr : acc[i - 1] + curr);
        return acc;
    }, []);
    
    paymentChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Loan Amount + Interest',
                    data: loanPlusInterest,
                    borderColor: '#ef4444',
                    backgroundColor: 'rgba(239, 68, 68, 0.1)',
                    fill: true
                },
                {
                    label: 'Cumulative Payments',
                    data: cumulativePayments,
                    borderColor: '#2563eb',
                    backgroundColor: 'rgba(37, 99, 235, 0.1)',
                    fill: true
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Loan Progress Over Time'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                label += new Intl.NumberFormat('de-DE', {
                                    style: 'currency',
                                    currency: 'EUR',
                                    minimumFractionDigits: 2,
                                    maximumFractionDigits: 2
                                }).format(context.parsed.y);
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return new Intl.NumberFormat('de-DE', {
                                style: 'currency',
                                currency: 'EUR',
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2
                            }).format(value);
                        }
                    }
                }
            }
        }
    });
};

window.exportToExcel = function (data) {
    const wb = XLSX.utils.book_new();
    
    const summaryData = [
        ['Loan Calculator Summary'],
        [''],
        ['Loan Amount', data.loanAmount],
        ['Interest Rate', data.interestRate],
        ['Loan Term', data.loanTerm],
        ['Additional Payment', data.additionalPayment],
        [''],
        ['Monthly Payment', data.monthlyPayment],
        ['Total Interest', data.totalInterest],
        ['Total Payment', data.totalPayment]
    ];
    
    const summaryWs = XLSX.utils.aoa_to_sheet(summaryData);
    XLSX.utils.book_append_sheet(wb, summaryWs, 'Summary');
    
    const monthlyData = [
        ['Month', 'Payment', 'Principal', 'Interest', 'Remaining Balance']
    ];
    
    data.monthlyPayments.forEach(payment => {
        monthlyData.push([
            payment.Month,
            payment.Payment,
            payment.Principal,
            payment.Interest,
            payment.RemainingBalance
        ]);
    });
    
    const monthlyWs = XLSX.utils.aoa_to_sheet(monthlyData);
    XLSX.utils.book_append_sheet(wb, monthlyWs, 'Monthly Breakdown');
    
    XLSX.writeFile(wb, 'loan-calculator.xlsx');
}; 