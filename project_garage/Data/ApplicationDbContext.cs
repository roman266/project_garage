using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using project_garage.Models.DbModels;
using Microsoft.Extensions.Hosting;
using System;
using static System.Collections.Specialized.BitVector32;

namespace project_garage.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<FriendModel> Friends { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<ConversationModel> Conversations { get; set; }
        public DbSet<PostImageModel> PostImages { get; set; }
        public DbSet<ReactionModel> ReactionActions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Викликаємо базову конфігурацію Identity

            // Налаштування таблиці Comment
            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.Post) 
                .WithMany(p => p.Comments) 
                .HasForeignKey(c => c.PostId) 
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.User) 
                .WithMany(u => u.Comments) 
                .HasForeignKey(c => c.UserId) 
                .OnDelete(DeleteBehavior.Restrict); 

            // Налаштування таблиці Post
            modelBuilder.Entity<PostModel>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostImageModel>()
                .HasOne(pi => pi.Post)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Налаштування таблиці Friend
            modelBuilder.Entity<FriendModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FriendModel>()
                .HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConversationModel>()
               .HasOne(u1 => u1.User1)
               .WithMany()
               .HasForeignKey(u1 => u1.User1Id)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConversationModel>()
                .HasOne(u2 => u2.User2)
                .WithMany()
                .HasForeignKey(u2 => u2.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Conversation) // Зв'язок із діалогом
                .WithMany(c => c.Messages) // Діалог має багато повідомлень
                .HasForeignKey(m => m.ConversationId) // Зовнішній ключ
                .OnDelete(DeleteBehavior.Cascade); // Каскадне видалення, якщо видалено діалог

            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Sender) // Зв'язок із відправником
                .WithMany() // Користувач може відправляти багато повідомлень
                .HasForeignKey(m => m.SenderId) // Зовнішній ключ
                .OnDelete(DeleteBehavior.Restrict); // Забороняємо видалення користувача, якщо є повідомлення

            modelBuilder.Entity<ReactionModel>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReactionModel>()
            .HasIndex(r => new { r.EntityId, r.EntityType });

        }
    }
}