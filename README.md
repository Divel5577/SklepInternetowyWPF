SklepInternetowyWPF


A WPF desktop application (.NET Framework 4.7.2) for an online store with product browsing, cart, orders, PDF export and a “Wheel of Fortune” discount feature.



Features

Product Catalog: Browse, search and sort products by name or price.

Categories: Filter by category or view all.

Shopping Cart & Orders: Add to cart, place orders, view order history.

PDF Export: Generate styled PDFs for cart, order history and sales statistics.

Sales Statistics: Top-selling products list.

Wheel of Fortune: One daily spin for a global discount on product prices.

User Accounts: Login (admin/admin123, user/user123), role-based UI (admin panel).

Animations & Styles: Fade-in effects, hover button styling.



Project Structure


SklepInternetowyWPF/

├─ App.xaml(.cs)             Application entry and login flow  
├─ Views/                    XAML windows (Main, Login, Cart, History, Stats, Wheel)  
├─ ViewModels/               MVVM logic (ProductViewModel, UserViewModel)  
├─ Models/                   Entities (Product, Category, User, Order, CartItem)  
├─ Data/                     SQLite DbContext (AppDbContext)  
├─ Utils/                    Helpers (PdfExporter, charts)  
├─ Converters/               XAML converters (PathToImage, Visibility)  
├─ Validators/               Validation rules (Required, Positive)  
└─ Images/                   Static assets and icons



Setup


Clone
git clone https://github.com/Divel5577/SklepInternetowyWPF.git

Open in Visual Studio and restore NuGet packages.

Run (F5). On first launch you’ll see the login window.



Usage


Login: Use admin/admin123 or user/user123.

Browse: Filter and sort products.

Cart: Add items, review and place orders.

History: View past orders and export to PDF.

Stats: View/export top-selling products (admin).

Wheel: Spin once per day for a discount that applies until midnight.



Contributors


Patryk Dulkowski

Gracjan Czyżewski



License

MIT. See LICENSE file.
