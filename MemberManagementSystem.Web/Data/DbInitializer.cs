using System;
using System.Linq;
using MemberManagementSystem.Web.Helpers;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure DB exists and tables are created
            context.Database.EnsureCreated();

            // Seed Users if empty
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "admin",
                        PasswordHash = SecurityHelper.HashPassword("admin123"),
                        FullName = "System Administrator",
                        Email = "admin@organization.org",
                        Role = "Admin",
                        IsActive = true
                    },
                    new User
                    {
                        Username = "dataentry",
                        PasswordHash = SecurityHelper.HashPassword("entry123"),
                        FullName = "Data Entry Officer",
                        Email = "entry@organization.org",
                        Role = "DataEntry",
                        IsActive = true
                    },
                    new User
                    {
                        Username = "finance",
                        PasswordHash = SecurityHelper.HashPassword("finance123"),
                        FullName = "Accounts & Finance Manager",
                        Email = "finance@organization.org",
                        Role = "Finance",
                        IsActive = true
                    },
                    new User
                    {
                        Username = "viewer",
                        PasswordHash = SecurityHelper.HashPassword("viewer123"),
                        FullName = "Guest Viewer",
                        Email = "viewer@organization.org",
                        Role = "Viewer",
                        IsActive = true
                    }
                );
                context.SaveChanges();
            }

            // Seed Members if empty
            if (!context.Members.Any())
            {
                var members = new[]
                {
                    new Member
                    {
                        MemberCode = "MEM-1001",
                        FullName = "মোহাম্মদ রফিকুল ইসলাম",
                        FatherName = "মোঃ নুরুল ইসলাম",
                        MotherName = "মোছাঃ রাবেয়া খাতুন",
                        DateOfBirth = new DateTime(1988, 4, 12),
                        NationalId = "19881234567890123",
                        MobileNumber = "01711223344",
                        Email = "rafiqul@gmail.com",
                        Address = "বাড়ি ১২, রোড ৪, ধানমন্ডি",
                        District = "Dhaka",
                        Division = "Dhaka",
                        Status = "Active",
                        JoiningDate = new DateTime(2025, 1, 15),
                        Remarks = "প্রতিষ্ঠাতা আজীবন সদস্য"
                    },
                    new Member
                    {
                        MemberCode = "MEM-1002",
                        FullName = "তানজিলা আক্তার",
                        FatherName = "আব্দুল করিম",
                        MotherName = "মমতাজ বেগম",
                        DateOfBirth = new DateTime(1992, 8, 23),
                        NationalId = "19929876543210987",
                        MobileNumber = "01819876543",
                        Email = "tanzila@gmail.com",
                        Address = "গ্রাম: বরুড়া, থানা: কোতোয়ালী",
                        District = "Cumilla",
                        Division = "Chittagong",
                        Status = "Active",
                        JoiningDate = new DateTime(2025, 2, 10),
                        Remarks = "নিয়মিত সদস্য"
                    },
                    new Member
                    {
                        MemberCode = "MEM-1003",
                        FullName = "কামরুল হাসান সজীব",
                        FatherName = "সুলতান আহমেদ",
                        MotherName = "ফরিদা পারভীন",
                        DateOfBirth = new DateTime(1995, 11, 5),
                        NationalId = "19954567891234567",
                        MobileNumber = "01912345678",
                        Email = "kamrul@gmail.com",
                        Address = "হোল্ডিং নং ৪৫, জেল রোড",
                        District = "Sylhet",
                        Division = "Sylhet",
                        Status = "Active",
                        JoiningDate = new DateTime(2025, 3, 1),
                        Remarks = "কার্যনির্বাহী সদস্য"
                    },
                    new Member
                    {
                        MemberCode = "MEM-1004",
                        FullName = "ফারহানা রহমান স্মৃতি",
                        FatherName = "মাহবুবুর রহমান",
                        MotherName = "নাসিমা বেগম",
                        DateOfBirth = new DateTime(1990, 6, 18),
                        NationalId = "19901122334455667",
                        MobileNumber = "01611335577",
                        Email = "farhana@yahoo.com",
                        Address = "সেক্টর ৩, উত্তরা মডেল টাউন",
                        District = "Dhaka",
                        Division = "Dhaka",
                        Status = "Active",
                        JoiningDate = new DateTime(2025, 3, 12),
                        Remarks = "সাধারণ সদস্য"
                    },
                    new Member
                    {
                        MemberCode = "MEM-1005",
                        FullName = "আব্দুল্লাহ আল মামুন",
                        FatherName = "শামসুল হক",
                        MotherName = "আয়েশা খাতুন",
                        DateOfBirth = new DateTime(1985, 1, 30),
                        NationalId = "19856677889900112",
                        MobileNumber = "01715566778",
                        Email = "mamun@gmail.com",
                        Address = "মাস্টারপাড়া, জিরো পয়েন্ট",
                        District = "Rajshahi",
                        Division = "Rajshahi",
                        Status = "Inactive",
                        JoiningDate = new DateTime(2024, 11, 20),
                        Remarks = "ঠিকানা পরিবর্তন সংক্রান্ত কারণে স্থগিত"
                    },
                    new Member
                    {
                        MemberCode = "MEM-1006",
                        FullName = "শারমিন সুলতানা",
                        FatherName = "রেজাউল করিম",
                        MotherName = "জাহানারা বেগম",
                        DateOfBirth = new DateTime(1993, 9, 14),
                        NationalId = "19932233445566778",
                        MobileNumber = "01519988776",
                        Email = "sharmin@gmail.com",
                        Address = "বয়রা মেইন রোড",
                        District = "Khulna",
                        Division = "Khulna",
                        Status = "Active",
                        JoiningDate = new DateTime(2025, 4, 5),
                        Remarks = "নিয়মিত সদস্য"
                    }
                };

                context.Members.AddRange(members);
                context.SaveChanges();

                // Seed Payments
                var firstMember = members[0];
                var secondMember = members[1];
                var thirdMember = members[2];

                context.Payments.AddRange(
                    new Payment
                    {
                        MemberId = firstMember.MemberId,
                        InvoiceNo = "INV-202501-1001",
                        FeeType = "Registration Fee",
                        Amount = 5000,
                        PaymentDate = new DateTime(2025, 1, 15),
                        PaymentMethod = "bKash",
                        TransactionId = "BK987X123",
                        Remarks = "রেজিস্ট্রেশন ফি পরিশোধিত",
                        ReceivedBy = "admin"
                    },
                    new Payment
                    {
                        MemberId = firstMember.MemberId,
                        InvoiceNo = "INV-202502-1002",
                        FeeType = "Annual Fee",
                        Amount = 2000,
                        PaymentDate = new DateTime(2025, 2, 1),
                        PaymentMethod = "Bank Transfer",
                        TransactionId = "TXN554433",
                        Remarks = "বার্ষিক নবায়ন ফি",
                        ReceivedBy = "finance"
                    },
                    new Payment
                    {
                        MemberId = secondMember.MemberId,
                        InvoiceNo = "INV-202502-1003",
                        FeeType = "Registration Fee",
                        Amount = 5000,
                        PaymentDate = new DateTime(2025, 2, 10),
                        PaymentMethod = "Cash",
                        Remarks = "নগদ ক্যাশে জমা",
                        ReceivedBy = "finance"
                    },
                    new Payment
                    {
                        MemberId = thirdMember.MemberId,
                        InvoiceNo = "INV-202503-1004",
                        FeeType = "Registration Fee",
                        Amount = 5000,
                        PaymentDate = new DateTime(2025, 3, 1),
                        PaymentMethod = "Nagad",
                        TransactionId = "NGD776655",
                        Remarks = "নগদ অ্যাকাউন্টে পরিশোধ",
                        ReceivedBy = "admin"
                    }
                );

                // Seed Documents
                context.Documents.AddRange(
                    new Document
                    {
                        MemberId = firstMember.MemberId,
                        DocumentType = "National ID (NID)",
                        FileName = "rafiqul_nid.pdf",
                        FilePath = "/uploads/documents/sample_nid.pdf",
                        FileSize = 245120,
                        FileExtension = ".pdf",
                        UploadDate = new DateTime(2025, 1, 15),
                        Remarks = "জাতীয় পরিচয়পত্রের সত্যায়িত ফটোকপি"
                    },
                    new Document
                    {
                        MemberId = secondMember.MemberId,
                        DocumentType = "Passport",
                        FileName = "tanzila_passport.pdf",
                        FilePath = "/uploads/documents/sample_passport.pdf",
                        FileSize = 312000,
                        FileExtension = ".pdf",
                        UploadDate = new DateTime(2025, 2, 10),
                        Remarks = "পাসপোর্টের প্রথম দুই পাতার কপি"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}

