using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using UniversityProject.Core.Entities;
using UniversityProject.Core.Entities.BaseEntities;
using UniversityProject.Core.Entities.EntityLogs;
using UniversityProject.Infrastructure.Extensions;
using UniversityProject.Infrastructure.Healper.Acls;
using static UniversityProject.Core.Entities.Auth.IdentityModel;

namespace UniversityProject.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ISignInHelper _signInHelper) : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RouteLog> RouteLogs => Set<RouteLog>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockLedger> StockLedgers => Set<StockLedger>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<CustomerLedger> CustomerLedgers => Set<CustomerLedger>();
    // SALES
    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<SalesItem> SalesItems => Set<SalesItem>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<CustomerPayment> CustomerPayments => Set<CustomerPayment>();
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
    public DbSet<SupplierLedger> SupplierLedgers => Set<SupplierLedger>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.RelationConvetion();
        modelBuilder.DateTimeConvention();
        modelBuilder.DecimalConvention();
        modelBuilder.ConfigureDecimalProperties();
        modelBuilder.PluralzseTableNameConventions();
        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.LogTo(message => WriteSqlQueryLog(message));
        optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() }));
    }
    public override int SaveChanges()
    {
        Audit();      // Track changes for auditing
        AuditTrail(); // Log detailed changes
        return base.SaveChanges();
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Audit();
            AuditTrail();
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
      
    }
    private static void WriteSqlQueryLog(string query, StoreType storeType = StoreType.Output)
    {
        if (storeType == StoreType.Output)
            Debug.WriteLine(query);
        else if (storeType == StoreType.Db)
        {
            // store in db
        }
        else if (storeType == StoreType.File)
        {
            // store & append in file
            //new StreamWriter("mylog.txt", append: true);
        }

    }

    private void Audit()
    {
        long userId = 0;
        var now = DateTimeOffset.UtcNow;

        if (_signInHelper.IsAuthenticated)
            userId = (long)_signInHelper.UserId;

        foreach (var entry in base
            .ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State == EntityState.Added
                     || e.State == EntityState.Modified))
        {
            if (entry.State != EntityState.Added)
            {
                entry.Entity.ModifiedDate ??= now;
                entry.Entity.ModifiedBy ??= userId;
            }
            else
            {
                entry.Entity.CreatedBy = entry.Entity.CreatedBy != 0 ? entry.Entity.CreatedBy : userId;
                entry.Entity.CreatedDate = entry.Entity.CreatedDate == DateTimeOffset.MinValue ? now : entry.Entity.CreatedDate;
            }
        }
    }

    private void AuditTrail()
    {
        long userId = 0;

        if (_signInHelper.IsAuthenticated)
            userId = (long)_signInHelper.UserId;

        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity
                || entry.Entity is AuditLog
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAuditLog());
        }
    }
}
public enum StoreType
{
    Db,
    File,
    Output
}
