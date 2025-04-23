using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkItemState> WorkItemStates { get; set; }

        public DbSet<WorkItemTag> WorkItemTag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(a => a.Area).HasColumnType("varchar(200)");
                eb.Property(a => a.IterationPath).HasColumnName("Iteration_Path");
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
                        wit.Property(wit => wit.PublicationDate).HasDefaultValueSql("getutcdate()");
                    }
                    );

                eb.HasOne(wi => wi.WorkItemstate).WithMany().HasForeignKey(wi => wi.StateId);
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
                eb.HasKey(e => e.StateId);
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
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>()
                .HasOne(a => a.Address)
                .WithOne(u => u.User)
                .HasForeignKey<Address>(a => a.UserId);

            modelBuilder.Entity<WorkItemState>()
                .HasData(
                new WorkItemState() { StateId = 1, State = "To Do" },
                new WorkItemState() { StateId = 2, State = "Doing" },
                new WorkItemState() { StateId = 3, State = "Done" },
                new WorkItemState() { StateId = 4, State = "On Hold" },
                new WorkItemState() { StateId = 5, State = "Rejected" });

            modelBuilder.Entity<Tag>()
                .HasData(
                new Tag() { Id = 1, Value ="Web"},
                new Tag() { Id = 2, Value = "UI"},
                new Tag() { Id = 3, Value = "Desktop"},
                new Tag() { Id = 4, Value = "API"},
                new Tag() { Id = 5, Value = "Service"}
                );
        }
    }
}
