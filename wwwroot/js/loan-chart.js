window.createPaymentChart = function (payments, interestPayments, initialAmount) {
    const ctx = document.getElementById('paymentChart').getContext('2d');
    
    // Create labels for x-axis (months)
    const labels = Array.from({ length: payments.length }, (_, i) => `Month ${i + 1}`);
    
    // Calculate cumulative sums
    const cumulativePayments = payments.reduce((acc, curr, i) => {
        acc.push(i === 0 ? curr : acc[i - 1] + curr);
        return acc;
    }, []);

    // Calculate cumulative interest plus initial amount
    const cumulativeInterest = interestPayments.reduce((acc, curr, i) => {
        acc.push(i === 0 ? curr + initialAmount : acc[i - 1] + curr);
        return acc;
    }, []);
    
    // Create the chart
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Cumulative Amount Paid',
                    data: cumulativePayments,
                    borderColor: 'rgb(75, 192, 192)',
                    backgroundColor: 'rgba(75, 192, 192, 0.1)',
                    tension: 0.1,
                    fill: true
                },
                {
                    label: 'Total Amount to Pay (Principal + Interest)',
                    data: cumulativeInterest,
                    borderColor: 'rgb(255, 99, 132)',
                    backgroundColor: 'rgba(255, 99, 132, 0.1)',
                    tension: 0.1,
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
                    text: 'Cumulative Payment Breakdown'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.dataset.label}: $${context.raw.toFixed(2)}`;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Amount ($)'
                    },
                    ticks: {
                        callback: function(value) {
                            return '$' + value.toFixed(2);
                        }
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Month'
                    }
                }
            }
        }
    });
}; 