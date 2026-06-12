using Hostelfoodsystem.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hostelfoodsystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created and migrations are applied
            context.Database.Migrate();

            // Seed default Admin User first (independent of student logs)
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Name = "Admin Mess Manager",
                    Email = "admin@hostelfood.com",
                    PasswordHash = "admin123", // Using simple password plain/hash for demonstration
                    Role = "Admin"
                });
                context.SaveChanges();
            }

            // Look for any students. If empty, seed sample data.
            if (context.Students.Any())
            {
                return; // DB has already been seeded with students
            }

            var students = new List<Student>
            {
                new Student { StudentName = "Aravind Swamy", Department = "Computer Science", Year = 3, HostelBlock = "A-Block" },
                new Student { StudentName = "Bala Murugan", Department = "Mechanical Eng", Year = 2, HostelBlock = "B-Block" },
                new Student { StudentName = "Chithra Devi", Department = "Electrical Eng", Year = 1, HostelBlock = "A-Block" },
                new Student { StudentName = "Dinesh Kumar", Department = "Information Tech", Year = 4, HostelBlock = "C-Block" },
                new Student { StudentName = "Elango Selvan", Department = "Civil Eng", Year = 2, HostelBlock = "B-Block" },
                new Student { StudentName = "Fathima Begum", Department = "Electronics Eng", Year = 3, HostelBlock = "A-Block" },
                new Student { StudentName = "Ganesh Moorthy", Department = "Bio-Technology", Year = 3, HostelBlock = "C-Block" },
                new Student { StudentName = "Hari Haran", Department = "Computer Science", Year = 2, HostelBlock = "B-Block" },
                new Student { StudentName = "Indira Priyadarshini", Department = "Chemical Eng", Year = 1, HostelBlock = "A-Block" },
                new Student { StudentName = "Jeeva Rathinam", Department = "Aeronautical Eng", Year = 2, HostelBlock = "C-Block" }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            // Seed historical meal records for the last 14 days
            var mealRecords = new List<MealRecord>();
            var random = new Random();
            var mealTypes = new[] { "Breakfast", "Lunch", "Dinner" };

            // Average prepared and consumed rates for simulation
            var basePrepared = new Dictionary<string, int>
            {
                { "Breakfast", 140 },
                { "Lunch", 180 },
                { "Dinner", 160 }
            };

            for (int i = 14; i >= 1; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                foreach (var mealType in mealTypes)
                {
                    int prepared = basePrepared[mealType] + random.Next(-15, 15);
                    // Consumed count is usually around 80% to 92% of prepared
                    double consumptionPercentage = 0.82 + (random.NextDouble() * 0.12);
                    int consumed = (int)Math.Round(prepared * consumptionPercentage);
                    int waste = prepared - consumed;

                    mealRecords.Add(new MealRecord
                    {
                        Date = date,
                        MealType = mealType,
                        PreparedCount = prepared,
                        ConsumedCount = consumed,
                        WasteCount = waste
                    });
                }
            }

            context.MealRecords.AddRange(mealRecords);
            context.SaveChanges();

            // Seed attendance records for students for the last 14 days
            var attendances = new List<Attendance>();
            var seededStudents = context.Students.ToList();

            for (int i = 14; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                foreach (var student in seededStudents)
                {
                    // 85% chance a student is marked present
                    bool isPresent = random.NextDouble() < 0.85;
                    attendances.Add(new Attendance
                    {
                        StudentId = student.Id,
                        Date = date,
                        IsPresent = isPresent
                    });
                }
            }

            context.Attendances.AddRange(attendances);
            context.SaveChanges();
        }
    }
}
