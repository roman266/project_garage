using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using project_garage.Models.DbModels;

namespace project_garage.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<FriendModel> Friends { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<ConversationModel> Conversations { get; set; }
        //public DbSet<PostImageModel> PostImages { get; set; }
        public DbSet<ReactionModel> ReactionActions { get; set; }
        public DbSet<UserConversationModel> UserConversations { get; set; }
        public DbSet<UserInterestModel> UserInterests { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
        public DbSet<InterestModel> Interests { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // Налаштування таблиці Message
            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Налаштування таблиці Reaction
            modelBuilder.Entity<ReactionModel>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReactionModel>()
                .HasIndex(r => new { r.EntityId, r.EntityType });

            //Налаштування таблиці PostImage
            //modelBuilder.Entity<PostImageModel>()
            //    .HasKey(pi => pi.Id);

            //modelBuilder.Entity<PostImageModel>()
            //    .Property(pi => pi.ImageUrl)
            //    .IsRequired()
            //    .HasMaxLength(500);

            //modelBuilder.Entity<PostImageModel>()
            //    .HasOne(pi => pi.Post)
            //    .WithMany(p => p.Images)
            //    .HasForeignKey(pi => pi.PostId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConversationModel>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserConversations)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConversationModel>()
                .HasOne(uc => uc.Conversation)
                .WithMany(c => c.UserConversations)
                .HasForeignKey(uc => uc.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserInterestModel>()
                .HasOne(ui => ui.User)
                .WithMany(u => u.UserInterests)
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserInterestModel>()
                .HasOne(ui => ui.Interest)
                .WithMany()
                .HasForeignKey(ui => ui.InterestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshTokenModel>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}