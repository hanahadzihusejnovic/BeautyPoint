# BeautyPoint salon&shop

**BeautyPoint** is an e-commerce and service management web application designed for a salon and shop environment. 
It allows users to browse and purchase beauty products, book beauty treatments, leave reviews, and make payments securely.
The system has different user roles, including Admin, Employee, and Client.


## Features

### Client-Side Features:
- User Registration and Login (Token-based Authentication)
- Product Browsing with Filters
- Product Categories (Hair, Skin, Body)
- Shopping Cart with Add/Remove Products
- Stripe Integration for Secure Payments
- Light/Dark Mode Toggle
- Product Rating and Review System
- "Save for Later" option in Shopping Cart
- Product Details page with reviews


### Admin-Side Features:
- User and Product management
- CRUD operations for Reservations, Treatments and Payments
- Order management

> **Note:** The Employee role and the Treatment Reservation system are under development.

---

## Technologies Used:

### Frontend:
- Angular (Web framework)
- TypeScript
- HTML, CSS

### Backend:
- ASP.NET Core
- SQL Server Database
- Stripe API

### Others:
- Azure DevOps (CI/CD)

--- 

## Installation

To run the project locally, ensure you have:
- .NET SDK 8.0+
- Node.js 18+
- Angular CLI
- SQL Server

### Steps to run project locally:

1. **Clone the Project**

2. **Set up the Database**
    - Create a new database in SQL Server (BeautyPointGitHub)

3. **Backend Setup**
    - Restore NuGet Packages if needed
    - Run command: 
    ```bash 
    update-database
    ```
    - Run the backend and the database will automatically be populated with Seed data.

4. **Frontend Setup** 
     ```bash
     npm install
     npm start
     ```

> Backend: https://localhost:7137 
> Frontend: https://localhost:4200

---

## Testing Stripe Payments

1. Obtain your Stripe test API keys from the [Stripe Dashboard](https://dashboard.stripe.com/test/apikeys)

    - Secret Key (e.g., sk_test_...)
    - Publishable Key (e.g., pk_test_...)

2. Add the keys to `appsettings.Development.json` file in the backend root (do **not** commit this file). 

    ```json
    {
      "Stripe": {
        "SecretKey": "sk_test_your_secret_key",
        "PublishableKey": "pk_test_your_publishable_key"
      }
    }
    ```

3. Use Stripe test card details to simulate payment:

    - Card Number: `4242 4242 4242 4242`
    - Expiry Date: `1234`
    - CVC: `123`
    - Postal number: `12345`

> Stripe will simulate a real payment transaction, but no actual money will be charged.

---

## Role and Contributions

This project was developed as a group effort, with the following responsibilities:

- **Client-Side:** Implemented by me, including all client-side features.
- **Admin-Side:** Managed by another team member, including product and user management.
- **Employee-Side and Treatment Reservations:** In progress

---

## Test Login Credentials

**Client:**
- Username: `client`
- Password: `Client123!`

**Admin:**
- Username: `admin`
- Password: `Admin123!`

