using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext:IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
                                                UserRole,IdentityUserLogin<int>, IdentityRoleClaim<int>,
                                                IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> Options) : base(Options){}
        
        public DbSet<Value> Values { get; set; }


        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        // override the Like Mode To make the Proprarty of(LikerId+LikeeId) as Primay Key
        // we use this function with the (Many-to-Many) Relationship
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // configure the schema needed for Identity
            
            builder.Entity<UserRole>(userRole => 
            {
                userRole.HasKey(ur => new{ur.UserId, ur.RoleId});
                userRole.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur =>ur.RoleId).IsRequired();
                userRole.HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur =>ur.UserId).IsRequired();
            });


            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId}); // to make primary key

            builder.Entity<Like>()
                .HasOne(u => u.Likee) // being liked from other
                .WithMany(u => u.Likers) // the likers
                .HasForeignKey(u => u.LikeeId) // save in the LikeeId Cloum in Db as FK
                .OnDelete(DeleteBehavior.Restrict); // Restricit becase we dont want when the user delete the like delete also the user

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}