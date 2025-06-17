# Hirafeyat - Handcrafted Products Marketplace

*Hirafeyat* is a full-featured web application built with **ASP.NET Core MVC**, designed to support an online marketplace for handcrafted products. It supports three main roles: **Admin**, **Seller**, and **Customer** — each with distinct permissions and functionality.  
The system is built on **ASP.NET Core Identity** for secure authentication, user management, and role-based access control.

---

 👥 User Roles

1. Admin
- Reviews and approves or rejects products submitted by sellers.
- Manages all users (Sellers and Customers).
- Full control over the platform.

2. Seller
- Adds and manages their own products.
- Monitors orders placed for their products.
- Updates order statuses (e.g., Processing, Shipped, Delivered).

3. Customer
- Browses and searches products.
- Adds products to Cart or Favorites.
- Places orders and pays securely via Stripe.
- Tracks order status (Pending, Shipped, Delivered).

---

✨ Main Features

- Role-based authentication and authorization using ASP.NET Core Identity.
- Secure login and registration for multiple user roles.
- Product approval workflow managed by Admin.
- Shopping Cart and Favorites functionality.
- Order management with detailed status tracking.
- Stripe integration for secure online payments.
- User profile and account management.
- Admin and Seller dashboards.

---

🛠️ Tech Stack

- **ASP.NET Core MVC**
- **ASP.NET Core Identity**
- **Entity Framework Core**
- **SQL Server**
- **Stripe Payment API**
- **Bootstrap / HTML / CSS / JavaScript**

---

 🧩 Database Structure Overview

 `ApplicationUser` (Inherits from `IdentityUser`)
- Stores user details like FullName, Address, ProfileImage, and AccountCreatedDate.
- Extended from ASP.NET Core Identity for full authentication and user management.
- Relationships:
  - One user → many `Products`
  - One user → many `Orders`
  - One user → many `CartItems`
  - One user → many `Favorites`

---

`Product`
- Added by sellers, reviewed by admin.
- Properties: Title, Description, Price, Image, Quantity, Category, Status.
- Relationships:
  - One product → one `Category`
  - One product → one `Seller` (ApplicationUser)
  - One product → many `OrderItems`, `CartItems`, and `Favorites`

---

`Category`
- Holds multiple products under a single classification.

---

`CartItem`
- Represents items a user has added to their shopping cart.
- Each item includes product, quantity, user, and timestamp.

---

`Favorite`
- Stores products the customer marked as favorite.
- Tied to both `User` and `Product`.

---

`Order`
- Represents a customer’s full purchase transaction.
- Properties: OrderDate, Address, Contact Info, Status.
- Relationships:
  - One order → many `OrderItems`
  - One order → one `Payment`
  - One customer → many `Orders`

---

`OrderItem`
- Represents each individual product inside an order.
- Tracks the quantity and status of each product separately.

---

 `Payment`
- Stores Stripe transaction details.
- Properties: Amount, Method, PaymentDate, Status.
- Linked to one `Order`.

---

🔄 Entity Relationships

```plaintext
ApplicationUser
 ├── 1 : * Products
 ├── 1 : * Orders
 ├── 1 : * CartItems
 └── 1 : * Favorites

Product
 ├── * : 1 Category
 ├── * : 1 Seller
 ├── 1 : * OrderItems
 ├── 1 : * CartItems
 └── 1 : * Favorites

Order
 ├── 1 : * OrderItems
 └── 1 : 1 Payment

OrderItem
 ├── * : 1 Product
 └── * : 1 Order


🧪 How to Run Locally
1- Clone the repository:
git clone https://github.com/AbuHussien28/Hirafeyaatt.git
1- Open the solution in Visual Studio.

3- Update appsettings.json with your SQL Server connection string.

4- Run migrations:
Update-Database