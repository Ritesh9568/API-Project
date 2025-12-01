using CoreBankingAPI.Models;
using CoreBankingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Data;

public partial class WebDbContext : DbContext
{
    public WebDbContext()
    {
    }

    public WebDbContext(DbContextOptions<WebDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Bankuser> Bankusers { get; set; }

    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CoreBankingDB;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Accountnumber).HasName("pk_account");

            entity.ToTable("account");

            entity.Property(e => e.Accountnumber)
                .HasMaxLength(15)
                .HasColumnName("accountnumber");
            entity.Property(e => e.Accounttype)
                .HasMaxLength(20)
                .HasColumnName("accounttype");
            entity.Property(e => e.CurrentBalance)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("currentbalance");
            entity.Property(e => e.Customer)
                .HasMaxLength(10)
                .HasColumnName("customerid_fk");
            entity.Property(e => e.Lastupdated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("lastupdated");
            entity.Property(e => e.Opendate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("opendate");
            entity.Property(e => e.OverdraftLimit)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("overdraftlimit");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");

            entity.HasOne(d => d.CustomeridFkNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_account_customer");
        });

        modelBuilder.Entity<Bankuser>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("pk_bankuser");

            entity.ToTable("bankuser");

            entity.HasIndex(e => e.Username, "uk_bankuser_username").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Createdon)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createdon");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Lastloggedin).HasColumnName("lastloggedin");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.HasKey(e => e.Beneficiaryid).HasName("pk_beneficiary");

            entity.ToTable("beneficiary");

            entity.HasIndex(e => new { e.CustomeridFk, e.Beneficiaryaccountnum, e.Beneficiaryifsc }, "uk_beneficiary_per_customer").IsUnique();

            entity.Property(e => e.Beneficiaryid).HasColumnName("beneficiaryid");
            entity.Property(e => e.Addedon)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("addedon");
            entity.Property(e => e.Beneficiaryaccountnum)
                .HasMaxLength(15)
                .HasColumnName("beneficiaryaccountnum");
            entity.Property(e => e.Beneficiaryifsc)
                .HasMaxLength(15)
                .HasColumnName("beneficiaryifsc");
            entity.Property(e => e.Beneficiaryname)
                .HasMaxLength(100)
                .HasColumnName("beneficiaryname");
            entity.Property(e => e.CustomeridFk)
                .HasMaxLength(10)
                .HasColumnName("customerid_fk");

            entity.HasOne(d => d.CustomeridFkNavigation).WithMany(p => p.Beneficiaries)
                .HasForeignKey(d => d.CustomeridFk)
                .HasConstraintName("fk_beneficiary_customer");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Branchid).HasName("pk_branch");

            entity.ToTable("branch");

            entity.HasIndex(e => e.Ifsccode, "uk_branch_ifsccode").IsUnique();

            entity.Property(e => e.Branchid)
                .HasMaxLength(6)
                .HasColumnName("branchid");
            entity.Property(e => e.Branchname)
                .HasMaxLength(100)
                .HasColumnName("branchname");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasDefaultValueSql("'India'::character varying")
                .HasColumnName("country");
            entity.Property(e => e.Dateopened)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("dateopened");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(15)
                .HasColumnName("ifsccode");
            entity.Property(e => e.Lastupdated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("lastupdated");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("pk_customer");

            entity.ToTable("customer");

            entity.HasIndex(e => e.Mobilenumber, "uk_customer_mobile").IsUnique();

            entity.Property(e => e.Customerid)
                .HasMaxLength(10)
                .HasColumnName("customerid");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Dateofbirth).HasColumnName("dateofbirth");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Kycstatus)
                .HasMaxLength(10)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("kycstatus");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Lastupdated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("lastupdated");
            entity.Property(e => e.Mobilenumber)
                .HasMaxLength(15)
                .HasColumnName("mobilenumber");
        });



        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Transactionid).HasName("pk_transaction");

            entity.ToTable("transaction");

            entity.HasIndex(e => e.Referencenumber, "uk_transaction_reference").IsUnique();

            entity.Property(e => e.Transactionid).HasColumnName("transactionid");
            entity.Property(e => e.AccountidFk)
                .HasMaxLength(15)
                .HasColumnName("accountid_fk");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Referencenumber)
                .HasMaxLength(50)
                .HasColumnName("referencenumber");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.Transactiontimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("transactiontimestamp");
            entity.Property(e => e.Transactiontype)
                .HasMaxLength(20)
                .HasColumnName("transactiontype");

            entity.HasOne(d => d.AccountidFkNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountidFk)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_transaction_account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
