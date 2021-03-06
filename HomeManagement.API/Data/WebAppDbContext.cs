﻿using HomeManagement.API.Data.Entities;
using HomeManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace HomeManagement.API.Data
{
    public class WebAppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<WebClient> WebClients { get; set; }

        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Preferences> Preferences { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<StorageItem> StorageItems { get; set; }

        public DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }

        public DbSet<MonthlyExpense> MonthlyExpenses { get; set; }

        public bool Disposed { get; set; }

        public WebAppDbContext(DbContextOptions<WebAppDbContext> options)
            : base(options)
        {
        }

        public override void Dispose()
        {
            Disposed = true;
            base.Dispose();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(x => x.Id);

            modelBuilder.Entity<WebClient>().HasKey(x => x.Id);

            modelBuilder.Entity<Account>().HasOne(x => x.User).WithMany(x => x.Accounts).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Transaction>().HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountId);

            modelBuilder.Entity<Transaction>().HasOne(x => x.Category);            

            modelBuilder.Entity<Category>().HasKey(x => x.Id);

            modelBuilder.Entity<Category>().HasOne(x => x.User).WithMany(x => x.Categories).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Reminder>().HasKey(x => x.Id);

            modelBuilder.Entity<Reminder>().HasOne(x => x.User);

            modelBuilder.Entity<Notification>().HasKey(x => x.Id);

            modelBuilder.Entity<Notification>().HasOne(x => x.Reminder);

            modelBuilder.Entity<Preferences>().HasOne(x => x.User);

            modelBuilder.Entity<ConfigurationSetting>().ToTable(nameof(ConfigurationSettings)).HasKey(x => x.Id);

            modelBuilder.Entity<MonthlyExpense>().HasKey(x => x.Id);

            modelBuilder.Entity<MonthlyExpense>().HasOne(x => x.User);

            modelBuilder.Ignore<Share>();
            modelBuilder.Ignore<Tax>();
            modelBuilder.Ignore<Token>();
        }
    }
}
