# Mobile Bill Generator

### Introduction:

This project is about developing a mobile billing engine (a simple class with a method to do the bill generation E.g GenerateBills) that can generate the monthly bill using call detail records (CDR). Assume that the engine is provided with a list of mobile customers and a list of CDRs that contains all the details of the calls made by these customers in the given billing month. The engine should generate the bills for all the customers. This bill should include the call charges for each of the calls made by the customer, total call charges for the month, monthly rental, government tax, discount (when applicable) and the final bill amount.

A customer object includes the below details,

- Full name
- Billing address
- Phone number
- Package Code
- Registered date

A CDR includes below details,

- Phone number of the subscriber originating the call ( [calling party](https://en.wikipedia.org/wiki/Calling_party))
- Phone number receiving the call ( [called party](https://en.wikipedia.org/wiki/Called_party))
- Starting time of the call (date and time)
- Call duration in seconds

Bill (This will be a simple class) should include below details:

- Customer full name
- Phone Number
- Billing Address
- Total call charges
- Total Discount
- Tax
- Rental
- Bill amount (Total Call Charges + Rental + Tax – Total Discount)
- List of call details:
  - Start time
  - Duration in seconds
  - Destination number
  - Charge

Phone numbers are given in below format:  xxx-xxxxxxx where first 3 digits represent the extension and the last 7 digits represent the unique phone number within that extension.

The billing is based on the package purchased by the customer, call destination (local / long distance) and call time (peak/ off peak). The rates can be calculated per second or per minute usage.

- Local calls – calls where the originating number and the receiving number has the same extension
- Long distance calls – calls where the originating number and the receiving number has different extensions
- Peak hours – 8:00 am to 8:00 pm (excluding 8:00 pm)
- Off peak hours – 8:00 pm to 8:00 am (excluding 8:00 am)

Note: International Direct Dialing (IDD) calls are out of scope for this version of the invoicing engine.

The government tax will be 20% of the total bill excluding any discounts and taxes (total call charges + monthly rental).

Step 1:

Develop a billing engine that generate the monthly bill using below rules.

Note: All figures are given in LKR

- Monthly Rental – 100
- Billing Type – Per minute

| **Call type** | **Per minute charge** | |
| --- | --- |---|
|| **Peak Hours** | **Off-peak hours** |
| Local calls | 3 | 2 |
| Long distance calls | 5 | 4 |



Change Request 1:

Let the billing charges and the billing type depend on the package. Below are the two packages that needs to be supported.

| **Package** | **Monthly Rental** | **Billing Type** | **Call type** | **Per minute charge** ||
| --- | --- | --- | --- | --- | --- |
| ||||**Peak Hours** | **Off-peak hours** |
| Package A | 100 | Per minute | Local calls | 3 | 2 |
|   |   |   | Long distance calls | 5 | 4 |
| Package B | 100 | Per second | Local calls | 4 | 3 |
|   |   |   | Long distance calls | 6 | 5 |



Change Request 2:

Add below two packages to the system.

| **Package** | **Monthly Rental** | **Billing Type** | **Call type** | **Per minute charge** ||
| --- | --- | --- | --- | --- | --- |
| ||||**Peak Hours** | **Off-peak hours** |
| Package C | 300 | Per minute | Local calls | 2 | 1 |
|   |   |   | Long distance calls | 3 | 2 |
| Package D | 300 | Per second | Local calls | 3 | 2 |
|   |   |   | Long distance calls | 5 | 4 |



Change Request 3:

- Change the peak and off-peak hours of Package A as
  - Peak hours – 10:00 am to 6:00 pm (excluding 6:00 pm)
  - Off peak hours – 6:00 pm to 10:00 am (excluding 10:00 am)
- Change the peak and off-peak hours of Package C as
  - Peak hours – 9:00 am to 6:00 pm (excluding 6:00 pm)
  - Off peak hours – 6:00 pm to 9:00 am (excluding 9:00 am)
- Make the first minute of all local off-peak calls free of charge for Package B
- Make first minute of all local calls free of charge for Package C
- Give 40% discount for Package A and Package B if the total call charges for the month exceeds 1000 LKR
