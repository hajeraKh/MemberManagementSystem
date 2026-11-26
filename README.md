# Member Management & Document Database System
**Visual Studio 2022 | ASP.NET Core 8.0 MVC | Microsoft SQL Server (Local SQLEXPRESS)**

ডকুমেন্ট: `D:\Jony\আপনার জন্য একটি Member Management.docx` এর উপর ভিত্তি করে সম্পূর্ণ প্রজেক্ট তৈরি করা হয়েছে।

---

## 🚀 দ্রুত শুরু করার নির্দেশিকা (Quick Start Guide)

### ১. Visual Studio 2022 এ প্রজেক্ট চালু করুন
1. ফোল্ডার ওপেন করুন: `D:\Jony\MemberManagementSystem`
2. **`MemberManagementSystem.sln`** ফাইলটিতে ডাবল ক্লিক করুন।
3. Visual Studio 2022 ওপেন হলে উপরে থাকা সবুজ **Play বাটন (https / IIS Express)** বা কীবোর্ডে **`F5`** / **`Ctrl + F5`** চাপুন।
4. ব্রাউজারে স্বয়ংক্রিয়ভাবে প্রজেক্টটি চালু হয়ে যাবে: `https://localhost:7281` বা `http://localhost:5157`।

---

## 🗄️ লোকাল ডাটাবেইজ তথ্য (Local Database Details)

- **SQL Server Instance:** `.\SQLEXPRESS` (আপনার লোকাল SQL Server 2019 Express)
- **Database Name:** `MemberManagementDb`
- **Authentication:** Windows Authentication (Trusted Connection)
- **Connection String:**
  ```json
  "Server=.\\SQLEXPRESS;Database=MemberManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  ```
- **ডাটাবেইজ স্ক্রিপ্ট ফাইল:** `D:\Jony\MemberManagementSystem\Database\create_database.sql`
  *(ডাটাবেইজ, সকল টেবিল এবং প্রাথমিক ডেমো ডাটা ইতিমধ্যে লোকাল SQLEXPRESS-এ তৈরি ও সিড করা আছে)*

---

## 🔑 ব্যবহারকারী লগইন ও রোল (Login Accounts & Roles)

লগইন পেজে সুবিধা অনুযায়ী **১-ক্লিক লগইন (Quick Demo Switcher)** রয়েছে। এছাড়াও নিচের ইউজারনেম ও পাসওয়ার্ড ব্যবহার করতে পারেন:

| রোল (Role) | ইউজারনেম | পাসওয়ার্ড | অনুমতি ও ক্ষমতা (Permissions) |
|---|---|---|---|
| **Admin** | `admin` | `admin123` | সম্পূর্ণ নিয়ন্ত্রণ (Full Access, Add, Edit, Delete) |
| **Data Entry** | `dataentry` | `entry123` | সদস্য নিবন্ধন, তথ্য আপডেট ও ডকুমেন্ট আপলোড |
| **Finance** | `finance` | `finance123` | পেমেন্ট এন্ট্রি, মানি রসিদ ও আর্থিক রিপোর্ট |
| **Viewer** | `viewer` | `viewer123` | তথ্য শুধু দেখার ও রিপোর্ট প্রিন্ট করার সুবিধা |

---

## 📦 প্রজেক্টে বিদ্যমান প্রধান মডিউলসমূহ

### ১. সদস্য তথ্য (Members Module)
- **সদস্য আইডি (Member ID):** স্বয়ংক্রিয়ভাবে জেনারেট হওয়া ইউনিক আইডি (যেমন: `MEM-1001`)
- **ফিল্ডসমূহ:** পূর্ণ নাম, পিতার নাম, মাতার নাম, জন্ম তারিখ, জাতীয় পরিচয়পত্র (NID), মোবাইল নম্বর, ইমেইল, পূর্ণ ঠিকানা, জেলা, বিভাগ, অবস্থা (Active/Inactive), যোগদানের তারিখ।
- **ছবি আপলোড (Photo Upload):** লাইভ প্রিভিউ সহ JPG/PNG ছবি আপলোড ও প্রোফাইলে প্রদর্শন।
- **স্ট্যাটাস টগল:** এক ক্লিকে Active থেকে Inactive এবং Inactive থেকে Active করার সুবিধা।
- **আইডি কার্ড প্রিন্ট (Member ID Badge):** সদস্যের জন্য আকর্ষণীয় ডিজিটাল মেম্বারশিপ কার্ড প্রিন্ট করার সুবিধা।

### ২. ডকুমেন্ট আপলোড (Document Repository)
- **ডকুমেন্টের ধরন:** National ID (NID), Passport, Birth Certificate, Educational Certificate, Photo, Other ইত্যাদি।
- **ফাইল সাপোর্ট:** PDF, JPG, PNG, DOCX।
- **ফিচার:** সরাসরি সদস্যের প্রোফাইল বা ডকুমেন্ট মেনু থেকে আপলোড, প্রিভিউ ও ডাউনলোড সুবিধা।

### ৩. পেমেন্ট ও ফি আদায় (Payments & Invoices)
- **ফি এর ধরন:** Registration Fee, Monthly Fee, Annual Fee, Renewal Fee, Donation, Other ইত্যাদি।
- **পেমেন্ট মেথড:** Cash, bKash, Nagad, Rocket, Bank Transfer, Cheque।
- **মানি রসিদ (Invoice / Money Receipt):** স্বয়ংক্রিয় রসিদ নং জেনারেট এবং অফিসিয়াল মানি রসিদ ভাউচার প্রিন্ট করার সুবিধা।
- রসিদ বা ব্যাংক স্লিপ সংযুক্ত করার ফাইল আপলোড সুবিধা।

### ৪. সার্চ সুবিধা (Advanced Search)
- নাম দ্বারা অনুসন্ধান (Search by Name)
- মোবাইল নম্বর দ্বারা অনুসন্ধান (Search by Mobile)
- সদস্য আইডি দ্বারা অনুসন্ধান (Search by Member Code)
- জেলা ও বিভাগ ভিত্তিক অনুসন্ধান (District & Division Filter)
- সদস্যের অবস্থা ভিত্তিক ফিল্টার (Active / Inactive)

### ৫. রিপোর্ট মডিউল (Reports & Analytics)
১. **সদস্য তালিকা রিপোর্ট (Member Directory):** প্রিন্ট ও CSV এক্সপোর্ট সহ।  
২. **নতুন সদস্য রিপোর্ট (New Members):** গত ৭, ৩০, ৯০ দিন বা ১ বছরের তালিকা।  
৩. **বকেয়া ফি রিপোর্ট (Fee Status & Outstanding):** কারা ফি দিয়েছেন এবং কার বকেয়া রয়েছে তার নিরীক্ষা।  
৪. **জেলা ভিত্তিক রিপোর্ট (District-wise Breakdown):** কোন জেলায় কতজন সদস্য এবং সক্রিয়/নিষ্ক্রিয় অনুপাত।  
৫. **ডকুমেন্ট আপলোড রিপোর্ট (Document Archive):** আপলোডকৃত সকল ডকুমেন্টের সামারি ও তালিকা।  
৬. **মাসিক আর্থিক রিপোর্ট (Monthly Financial):** ১২ মাসের মাসওয়ারি আয় ও ফি ক্যাটাগরি বিশ্লেষণ।  
*(প্রতিটি রিপোর্টে রয়েছে **Print** ও **CSV Export** সুবিধা)*

### ৬. ড্যাশবোর্ড (Interactive Dashboard)
- মোট সদস্য, সক্রিয় সদস্য, নতুন সদস্য, মোট ফি আদায়ের লাইভ স্ট্যাটাস কার্ড।
- **Chart.js** দ্বারা তৈরি ৬ মাসের মাসিক ফি আদায়ের ট্রেন্ড গ্রাফ (Bar Chart)।
- জেলা ভিত্তিক সদস্য বণ্টনের চার্ট (Doughnut Chart)।
- সাম্প্রতিক ৫ জন সদস্য, ৫টি পেমেন্ট এবং ৫টি ডকুমেন্টের তাৎক্ষণিক তালিকা।

---

## 🛠️ ফোল্ডার স্ট্রাকচার

```
D:\Jony\MemberManagementSystem\
│
├── MemberManagementSystem.sln            <-- Visual Studio Solution File
├── Database\
│   └── create_database.sql               <-- Local SQL Server T-SQL script
│
├── MemberManagementSystem.Web\           <-- ASP.NET Core 8.0 MVC Project
│   ├── Controllers\                      <-- MVC Controllers
│   │   ├── AccountController.cs
│   │   ├── HomeController.cs
│   │   ├── MembersController.cs
│   │   ├── DocumentsController.cs
│   │   ├── PaymentsController.cs
│   │   ├── SearchController.cs
│   │   └── ReportsController.cs
│   │
│   ├── Models\                           <-- Entity Models
│   │   ├── Member.cs
│   │   ├── Document.cs
│   │   ├── Payment.cs
│   │   ├── User.cs
│   │   └── ViewModels\                   <-- View Models
│   │
│   ├── Data\                             <-- EF Core DbContext & Seeder
│   │   ├── ApplicationDbContext.cs
│   │   └── DbInitializer.cs
│   │
│   ├── Services\                         <-- File Upload & Helpers
│   │   ├── IFileService.cs
│   │   └── FileService.cs
│   │
│   ├── Views\                            <-- Razor Views
│   │   ├── Shared\_Layout.cshtml
│   │   ├── Account\ (Login, AccessDenied)
│   │   ├── Home\ (Dashboard with Charts)
│   │   ├── Members\ (Index, Create, Edit, Details, Card)
│   │   ├── Documents\ (Index, Upload)
│   │   ├── Payments\ (Index, Create, Invoice)
│   │   ├── Search\ (Advanced Search)
│   │   └── Reports\ (6 Standard Reports)
│   │
│   ├── wwwroot\uploads\                  <-- Uploaded files (Photos, Documents, Receipts)
│   ├── appsettings.json                  <-- DB Connection String
│   └── Program.cs
│
└── README.md
```

