using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options) { }

        DbSet<Address> Addresses { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<WorkItem> WorkItems { get; set; }
        DbSet<Issue> Issues { get; set; }
        DbSet<Epic> Epics { get; set; }
        DbSet<Task> Tasks { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<WorkItemState> WorkItemStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(a => a.Area).HasColumnType("varchar(200)");
                eb.Property(a => a.IterationPath).HasColumnName("Iternation_Path");
                eb.HasMany(c => c.Comments).WithOne(wi => wi.WorkItem).HasForeignKey(c => c.WorkItemId);
                eb.HasOne(a => a.Author).WithMany(u => u.WorkItems).HasForeignKey(a => a.AuthorId);

                eb.HasMany(t => t.Tags).WithMany(wi => wi.WorkItems).UsingEntity<WorkItemTag>(
                    w => w.HasOne(wit => wit.Tag)
                    .WithMany()
                    .HasForeignKey(wit => wit.TagId),

                    w => w.HasOne(wit => wit.WorkItem)
                    .WithMany()
                    .HasForeignKey(wit => wit.WorkItemId),

                    wit =>
                    {
                        wit.HasKey(wit => new { wit.WorkItemId, wit.TagId });
                        wit.Property(wit => wit.PublicateDate).HasDefaultValueSql("getutcdate()");
                    }
                    );

                eb.HasOne(wi => wi.WorkItemstate).WithMany().HasForeignKey(wi => wi.WorkItemStateId);
            });

            modelBuilder.Entity<Epic>()
                .Property(ed => ed.EndDate).HasPrecision(3);

            modelBuilder.Entity<Issue>()
                .Property(e => e.Efford).HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Task>(eb =>
            {
                eb.Property(a => a.Activity).HasMaxLength(200);
                eb.Property(a => a.RemaningWork).HasPrecision(14, 2);
            });

            modelBuilder.Entity<WorkItemState>(eb =>
            {
                eb.Property(e => e.State).IsRequired();
                eb.Property(e => e.State).HasMaxLength(50);
            });

            modelBuilder.Entity<Comment>(eb =>
            {
                eb.Property(cd => cd.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(ed => ed.UpdatedDate).ValueGeneratedOnUpdate();
                eb.HasOne(c => c.Author)
                    .WithMany(a => a.Comments)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<User>()
                .HasOne(a => a.Address)
                .WithOne(u => u.User)
                .HasForeignKey<Address>(a => a.UserId);

        }
    }
}
