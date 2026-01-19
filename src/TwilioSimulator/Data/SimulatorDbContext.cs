using Microsoft.EntityFrameworkCore;
using TwilioSimulator.Models;

namespace TwilioSimulator.Data;

public class SimulatorDbContext : DbContext
{
    public SimulatorDbContext(DbContextOptions<SimulatorDbContext> options) : base(options)
    {
    }

    public DbSet<SmsMessage> SmsMessages => Set<SmsMessage>();
    public DbSet<Conversation> Conversations => Set<Conversation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);

        modelBuilder.Entity<Conversation>()
            .HasIndex(c => c.PhoneNumber)
            .IsUnique();
    }
}
