using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;

namespace crud
{

    class Program
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class DbCrudContext : DbContext
        {
            public DbSet<User> users { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=CrudTest;Integrated Security=true");
                base.OnConfiguring(optionsBuilder);
            }

            public override int SaveChanges()
            {
                foreach (var obj in ChangeTracker.Entries())
                {
                    if(obj.State == EntityState.Deleted)
                    {
                        obj.State = EntityState.Modified;
                        //var name =   obj.CurrentValues["Name"];
                        obj.CurrentValues["IsDeleted"] = true;
                    }
                }
                return base.SaveChanges();
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>().HasQueryFilter(c => c.IsDeleted == false);
                base.OnModelCreating(modelBuilder);
            }
        }
        static void Main(string[] args)
        {
            var ctx = new DbCrudContext();
            //AddToDB(ctx);
            //updateWithId(ctx);
            //UpdateWithObj(ctx);

            SoftDeleted(ctx);

            Console.ReadKey();
        }

        private static void SoftDeleted(DbCrudContext ctx)
        {
            var request = new
            {
                Id = 5
            };

            var user = ctx.users.Find(request.Id);

            ctx.Remove(user);

            ctx.SaveChanges();
        }

        private static void UpdateWithObj(DbCrudContext ctx)
        {
            var request = new
            {
                Id = 3,
                Name = "Update with obj",
                IsDeleted = false
            };
            User user = new User
            {
                Id = request.Id,
                Name = request.Name,
                IsDeleted = request.IsDeleted
            };
            ctx.Update(user);
            ctx.SaveChanges();
        }

        private static void updateWithId(DbCrudContext ctx)
        {
            var request = new
            {
                Id = 2,
                Name = "Updated Name with Id"
            };
            var user = ctx.users.SingleOrDefaultAsync(c => c.Id == request.Id).Result;

            user.Name = request.Name;


            ctx.SaveChanges();
        }

        private static void AddToDB(DbCrudContext ctx)
        {
            
            List<User> users = new List<User>()
            {
                 new User
                {
                     Name = "reza",
                     IsDeleted = false,
                },
                 new User
                {
                     Name = "mohammad",
                     IsDeleted = false,
                },
                 new User
                {
                     Name = "ali",
                     IsDeleted = false,
                },
                 new User
                {
                     Name = "hamid",
                     IsDeleted = false,
                },
                 new User
                {
                     Name = "abbas",
                     IsDeleted = false,
                },

            };

            foreach (User user in users)
            {
                ctx.Add(user);
            }
            ctx.SaveChanges();

            Console.WriteLine("data added");
        }
    }
}
