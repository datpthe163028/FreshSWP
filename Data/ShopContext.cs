using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Reflection.Emit;
using System.Text;

namespace Project.Data
{
    public class ShopContext : IdentityDbContext
    {
        public ShopContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<ProductDetails> productdetails { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillDetail> BillDetails { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Payments> Payments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Transport> Transports { get; set; } = null!;
        public virtual DbSet<IdentityUser> Users { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<ImageProduct> ImageProducts { get; set; } = null!;
        public virtual DbSet<ImageBlog> ImageBlogs { get; set; } = null!;
        public virtual DbSet<SubCategory> SubCategory { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<WishList> WishList { get; set; } = null!;
        public virtual DbSet<Comment> Comment { get; set; } = null!;
        public DbSet<Answer> Answer { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.HasKey(s => s.SubCategoryId);
                entity.HasOne(s => s.Cateogory).
                       WithMany(c => c.SubCategories).HasForeignKey(c => c.CateogoryId)
                       .OnDelete(DeleteBehavior.Cascade);
                entity.Property(p => p.SubCategoryId).ValueGeneratedOnAdd();
                entity.Property(p => p.typeGender).IsRequired(false);

            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(s => s.commentId);

                entity.HasOne(d => d.Blog)
                    .WithMany(c => c.Comment)
                    .HasForeignKey(d => d.Blogid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bilaB5");

                entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);




            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(s => s.answerId);


                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.commentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bills__paB5");

                entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<WishList>(entity =>
            {
                entity.HasKey(s => s.WishListId);


            });




            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(s => s.CartId);
                entity.Property(a => a.UserId)
                     .HasColumnType("varchar(255)");

                entity.Property(p => p.CartId).ValueGeneratedOnAdd();

            });

            modelBuilder.Entity<ProductDetails>(entity =>
            {
                entity.HasKey(s => s.productDetailId);

            });


            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(s => s.CartItemId);
                entity.HasOne(s => s.cart).
                       WithMany(s => s.CartItems).HasForeignKey(c => c.CartId)
                       .OnDelete(DeleteBehavior.Cascade);
                entity.Property(p => p.CartItemId).ValueGeneratedOnAdd();


                entity.HasOne(s => s.product).
                     WithMany(s => s.CartItems).HasForeignKey(c => c.ProductId)
                     .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(b => b.Blogid);
                entity.Property(p => p.Blogid).ValueGeneratedOnAdd();
            });


            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.AddressId);
                entity.Property(a => a.UserId)
                  .HasColumnType("varchar(255)");
                entity.Property(p => p.AddressId).ValueGeneratedOnAdd();


            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.Property(e => e.UserId)
                  .IsRequired(false);
                entity.HasKey(b => b.BillId);
                entity.Property(p => p.BillId).ValueGeneratedOnAdd();
                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("date");

                entity.HasOne(d => d.PaymentCodeNavigation)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.PaymentCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bills__payment_c__38996AB5");

                entity.HasOne(d => d.Transport)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.TransportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bills__transport__3A81B327");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bills__user_id__398D8EEE");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(d => d.BillDetailId);
                entity.Property(p => p.BillDetailId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Bill)
                    .WithMany(d => d.BillDetails)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bill_deta__bill___3E52440B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bill_deta__produ__3F466844");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.FeedbackAnswer)
                  .IsRequired(false);
                entity.HasKey(f => f.FeedbackId);
                entity.Property(p => p.FeedbackId).ValueGeneratedOnAdd();
                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.FeedbackAnswer)
                    .HasMaxLength(250)
                    .HasColumnName("feedback_answer");

                entity.Property(e => e.FeedbackDate)
                    .HasColumnType("date")
                    .HasColumnName("feedback_date");

                entity.Property(e => e.FeedbackDetail)
                    .HasMaxLength(250)
                    .HasColumnName("feedback_detail");

                entity.Property(e => e.FeedbackStatus)
                    .HasMaxLength(50)
                    .HasColumnName("feedback_status");

                entity.Property(e => e.FeedbackTitle)
                    .HasMaxLength(50)
                    .HasColumnName("feedback_title");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__feedback__produc__403A8C7D");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__feedback__user_i__276EDEB3");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(p => p.CategoryId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasKey(e => e.PaymentCode);
                entity.Property(p => p.PaymentCode).ValueGeneratedOnAdd();

            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.ProductId).ValueGeneratedOnAdd();

                entity.Property(e => e.ImportDate)
                    .HasColumnType("date");
                entity.Property(p => p.BlogId).IsRequired(false);


                entity.HasOne(p => p.Blog)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(p => p.SubCategory)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubCategoryID).OnDelete(DeleteBehavior.Cascade);

            });





            modelBuilder.Entity<Transport>(entity =>
            {
                entity.HasKey(t => t.TransportId);
                entity.Property(p => p.TransportId).ValueGeneratedOnAdd();

            });

            modelBuilder.Entity<ImageBlog>(entity =>
            {
                entity.HasKey(i => i.ImageBlogId);
                entity.Property(p => p.ImageBlogId).ValueGeneratedOnAdd();
                entity.HasOne(i => i.Blog)
                        .WithMany(b => b.ImageBlogs)
                        .HasForeignKey(i => i.BlogId);
            });

            modelBuilder.Entity<ImageProduct>(entity =>
            {
                entity.HasKey(i => i.ImageProductId);
                entity.Property(p => p.ImageProductId).ValueGeneratedOnAdd();
                entity.HasOne(i => i.Product)
                        .WithMany(b => b.ImageProducts)
                        .HasForeignKey(i => i.ProductId);
            });



            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}
