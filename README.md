BeautyPoint salon&shop

BeautyPoint is an e-commerce and service management web application designed for a salon and shop environment. It allows users to browse and purchase beauty products, book beauty treatments, leave reviews, and make payments securely. The system has different user roles, including Admin, Employee, and Client.
Features
•	Client-Side Features:
o	User Registration and Login (Token-based Authentication)
o	Product Browsing with Filters
o	Product Categories (Hair, Skin, Body)
o	Shopping Cart with Add/Remove Products
o	Stripe Integration for Secure Payments
o	Light/Dark Mode Toggle
o	Product Rating and Review System
o	"Save for Later" option in Shopping Cart
o	Product Details page with reviews

•	Admin-Side Features:
o	User and Product management
o	CRUD operations for Reservations, Treatments and Payments
o	Order management
Note: The Employee role and the Treatment Reservation system are under development.

Technologies used:
•	Frontend:
o	Angular (Web framework)
o	TypeScript
o	HTML, CSS
•	Backend:
o	ASP.NET Core
o	SQL Server Database
o	Stripe API for Payments
•	Others:
o	Azure DevOps for continuous integration

Installation
To set up and run the project locally, make sure you have the following installed:
o	.NET SDK 8.0+
o	Node.js 18+
o	Angular CLI
o	SQL Server

Steps to run project locally:
1.	Clone the Project
2.	Set up the Frontend 
o	cd /frontend
o	npm install (dependencies)
o	npm start (running application)
3.	Set up the Backend
o	cd /backend
o	Restore NuGet Packages
4.	Set up the Database
o	Create a new database in SQL Server (BeautyPointGitHub)
5.	Seeder
o	The backend includes a seeder that will populate the database with initial data such as product categories and products. Run the backend, and the database will automatically be populated.

The backend should be accessible at https://localhost:7137. Local development server and the frontend will be accessible at https://localhost:4200.

Testing Stripe Payments
1. Obtain your Stripe test API keys from the Stripe Dashboard.
   Secret Key (e.g., sk_test_...)
   Publishable Key (e.g., pk_test_...)
2. Add the keys to your appsettings.Development.json file in the backend project root (do not commit this file). Example structure:
    "Stripe": {
        "SecretKey": "sk_test_your_secret_key",
        "PublishableKey": "pk_test_your_publishable_key"
    }
3. To test Stripe payment functionality, use the following Stripe test card details:
•	Card Number: 4242 4242 4242 4242
•	Expiry Date: 1234
•	CVC: 123
•	Postal number: 12345
Stripe will simulate a real payment transaction, but no actual money will be charged.

Role and Contributions
This project was developed as a group effort, with the following responsibilities:
•	Client-Side: Implemented by me, including all client-side features.
•	Admin-Side: Managed by another team member, including product and user management.
•	Employee-Side and Treatment Reservations: Not yet completed.

Test Login data
Client:
•	Username: client
•	Password: Client123!
Admin:
•	Username: admin
•	Password: Admin123!

