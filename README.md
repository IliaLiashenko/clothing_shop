# Clothing shop using ASP.NET Core MVC
This project was a **diploma project** for my college course, showcasing my skills in building full-stack web applications with **ASP.NET Core MVC**. While the project is not fully completed yet (for example, email sender functionality is missing), it already includes a variety of features, such as user authentication, a product catalog, shopping cart, and Stripe payment integration.

The project is containerized using **Docker** and can be easily run locally or deployed to production. The code is organized in an MVC architecture, making it scalable and maintainable for future updates.

You can view a short presentation with store interface screenshots and brief explanations [here](presentation.pdf).

<p>&nbsp;</p>

## Table of Contents
- [⚙️ Functionality](#-functionality)
- [🛠️ Tech Stack](#-tech-stack)
- [📂 Project Structure](#-project-structure)
- [🚀 Deployment](#-deployment)

<p>&nbsp;</p>

## ⚙️ Functionality

### Users & Authentication

- User registration  
- Login and authentication  
- Roles: User / Admin  
- Account management  
  - Only email and phone number stored  
  - Password change  
  - Two-factor authentication  
  - Uses built-in identity template form  

### Product Catalog

- Product listing  
- Product details page  
- Product categories  
- Product filtering  
- Product sorting  

### Cart & Checkout

- Add product to cart  
- Change quantity in cart  
- Remove product from cart  
- Place order  
- Payment via Stripe (test mode)  
- Store completed orders  

### Admin Panel

- Order management  
  - View, delete, sort orders  
- Product management (CRUD)  
- Manage categories, sizes, colors, brands, styles, genders (CRUD)  
- Create additional admin users

<p>&nbsp;</p>

## 🛠️ Tech Stack

- **ASP.NET Core MVC** – Backend and Web Interface  
- **Entity Framework Core** – ORM for working with the database  
- **SQL Server** – Relational Database  
- **MSSQL** – Database server used inside the Docker container  
- **Stripe API** – Integration for online payments (test mode)  
- **Docker & Docker Compose** – Containerization and deployment  
- **Bootstrap** – UI design and responsive layout

<p>&nbsp;</p>

## 📂 Project Structure
```plaintext
clothing_shop
├── clothing_shop
│   ├── appsettings.Development.json  # Configuration file for local development environment
│   ├── appsettings.Production.json   # Configuration file for production deployment, including Docker usage
│   ├── appsettings.json              # Base configuration
│   ├── clothing_shop.csproj          # Project file
│   ├── clothing_shop.csproj.user
│   ├── Program.cs   # Main entry point of the application; configures and runs the web server
│   ├── Dockerfile   # Instructions to build a Docker image for the application
│
├── Areas
│   └── Identity  # User authentication and Identity management
│
├── Controllers   # MVC Controllers handling the request logic
│
├── Properties    # Project properties and launch settings
│
├── Views         # Razor Views (UI templates)
│
├── wwwroot       # Static files (CSS, JS, images)
│
├── Shop_DataAccess
│   ├── Shop_DataAccess.csproj
│   ├── Data
│   │   └── ApplicationDbContext.cs   # Database context for Entity Framework
│   ├── Migrations
│   └── Repository    # Repository pattern for data access
│
├── Shop_Models
│   └── ViewModels    # View Models used to pass data to Views
│
├── Shop_Utility
│
├── Dockerfile.sqlserver    # Dockerfile for SQL Server container
├── docker-compose.yml      # Docker Compose configuration file
├── docker-compose.dcproj
├── launchSettings.json
├── .env                    # Environment variables
├── .dockerignore
└── docker_data
    └── ClothingShop.sql    # SQL script to initialize the database
```

<p>&nbsp;</p>

## 🚀 Deployment

### 1. Install Docker Desktop

Before you begin, make sure you have [Docker Desktop](https://www.docker.com/products/docker-desktop) installed. Docker Desktop is available for **Windows**, **macOS**, and **Linux** (with support for Docker Compose included).

### 2. Clone the Repository

Clone the repository to your local machine:

```bash
git clone https://github.com/IliaLiashenko/clothing_shop.git
cd clothing_shop
```

### 3. Create the .env File
Create a `.env` file in the root of the project to set your environment variables. You will need to insert your Stripe API keys, which you can get from Stripe Dashboard.

Add the following contents to your `.env` file:

```bash
STRIPE_SECRET_KEY=your_stripe_secret_key
STRIPE_PUBLISHABLE_KEY=your_stripe_publishable_key
```

### 4. Build and Run the Project
Run the project using Docker Compose. This will build the images for all services and start the containers.

```bash
docker compose up --build
```
After running this command, Docker Compose will create and start two containers:

- clothing_shop — The ASP.NET Core application.

- sqlserver — The MSSQL database with initial data imported from /docker_data/ClothingShop.sql.

### 5. The Application Will Be Available at
Once the containers are successfully up, your application will be available at:

👉 http://localhost:8080


### 6. Testing

During testing, when you attempt to place an order, the store will prompt you to create a user account. After registration, you will be able to fill in your payment information.

The Stripe payment system is operating in test mode, so you cannot use real card details. Instead, use the following test credentials:

- Card number: 4242 4242 4242 4242

- Expiration date (MM/YY): 02/42

- CVC: 242

To access the admin panel and test admin-specific functionalities, log in using:

- Email: testadmin@gmail.com

- Password: Password123


