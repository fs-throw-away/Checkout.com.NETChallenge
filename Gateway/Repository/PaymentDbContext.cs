using System;
using System.ComponentModel.DataAnnotations;
using Gateway.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Repository
{
    public class PaymentDbContext : DbContext
    {
        public DbSet<PaymentDbRecord> Payments { get; set; }

        public PaymentDbContext(DbContextOptions<PaymentDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentDbRecord>().ToTable("Payments");
        }
    }

    public class PaymentDbRecord
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public ulong CardNumber { get; set; }
        
        [Required]
        public int ExpiryMonth { get; set; }
        
        [Required]
        public int ExpiryYear { get; set; }
        
        [Required]
        public int Cvv { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string? Currency { get; set; }
        
        [Required]
        public DateTime InsertedDateTime { get; set; }

        public Guid? BankPaymentId { get; set; }
        
        public string? Status { get; set; }
        
        public DateTime? CompletedDateTime { get; set; }
    }
}