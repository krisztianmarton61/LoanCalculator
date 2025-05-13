# Loan Calculator

A Blazor WebAssembly application that helps users calculate and visualize loan payments, interest, and amortization schedules.

## Features

- Calculate monthly loan payments
- View total interest and principal payments
- Interactive payment schedule visualization
- Detailed monthly payment breakdown
- Support for additional monthly payments
- Responsive design for all devices
- Export functionality for payment schedules

## Technologies Used

- .NET 9.0
- Blazor WebAssembly
- Bootstrap 5.3.2
- Chart.js 4.0.1
- Bootstrap Icons

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- A code editor (Visual Studio, VS Code, etc.)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/krisztianmarton61/LoanCalculator.git
   ```

2. Navigate to the project directory:
   ```bash
   cd BlazorWasmApp
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

## Usage

1. Enter the loan details:
   - Loan Amount
   - Interest Rate
   - Loan Term (in years)
   - Optional: Additional Monthly Payment

2. Click "Calculate Loan" to see the results:
   - Monthly payment amount
   - Total amount to be paid
   - Number of months
   - Interactive payment schedule chart
   - Detailed monthly breakdown

3. Use the "Show Monthly Payment Breakdown" button to view detailed payment information for each month.

4. Export the payment schedule to Excel for further analysis.