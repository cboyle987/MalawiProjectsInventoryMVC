using MalawiProjectsInventoryMVC.Entities;
using Microsoft.EntityFrameworkCore;

namespace MalawiProjectsInventoryMVC.Context;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    
    public DbSet<Donor> Donors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("Donors");

        modelBuilder.Entity<Donor>().ToContainer("Donors");
        modelBuilder.Entity<Donor>().HasPartitionKey(d => d.Id);

        modelBuilder.Entity<Donor>().OwnsMany(d => d.Donations, donation =>
        {
            donation.WithOwner();

            donation.OwnsMany(d => d.DonatedItems, item =>
            {
                item.WithOwner();
            });
        });
    }
}