using Microsoft.EntityFrameworkCore;
using Reserve.Data.Elements;
using Reserve.Data.Extensions;
using Reserve.Data.Models;
using Reserve.Data.Traits;

namespace Reserve.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public DbSet<Page> Pages { get; set; }
        public DbSet<VisualElement> VisualElements { get; set; }
        
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceAssignee> ServiceAssignees { get; set; }
        public DbSet<ServiceLocation> ServiceLocations { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservedService> ReservedServices { get; set; }
        public DbSet<ReservationEvent> ReservationEvents { get; set; }
        
        public DbSet<GlobalSetting> GlobalSettings { get; set; }
        public DbSet<EmployeeSetting> EmployeeSettings { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {}
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Account>()
                .HasIndex(it => it.Email)
                .IsUnique();

            builder.Entity<VisualElement>()
                .HasIndex(it => it.Type);

            builder.Entity<Page>()
                .HasIndex(it => it.Identifier)
                .IsUnique();

            builder.Entity<ServiceCategory>()
                .HasOne(it => it.ParentCategory)
                .WithMany(it => it.ChildCategories)
                .HasForeignKey(it => it.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<ReservedService>()
                .HasKey(it => new
                {
                    it.ReservationId,
                    it.ServiceId
                });

            builder.Entity<Reservation>()
                .HasOne(it => it.Account)
                .WithMany(it => it.Reservations)
                .HasForeignKey(it => it.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(it => it.Assignee)
                .WithMany(it => it.AssignedReservations)
                .HasForeignKey(it => it.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.ConfigureTimestampedEntities();
            builder.UseUnderscoreNaming();
        }
    }
}