using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvcflowershoplab1.Areas.Identity.Data;

namespace mvcflowershoplab1.Data;

public class mvcflowershoplab1Context : IdentityDbContext<mvcflowershoplab1User>
{
    public mvcflowershoplab1Context(DbContextOptions<mvcflowershoplab1Context> options)
        : base(options)
    {
    }

    public DbSet<mvcflowershoplab1.Models.Bike> BikeTable { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
