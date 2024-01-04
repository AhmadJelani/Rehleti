using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Rehleti.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdventureTrip> AdventureTrips { get; set; }

    public virtual DbSet<BookChalet> BookChalets { get; set; }

    public virtual DbSet<BookTrip> BookTrips { get; set; }

    public virtual DbSet<Chalet> Chalets { get; set; }

    public virtual DbSet<ChaletFeedback> ChaletFeedbacks { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<ListOfDatesForChalet> ListOfDatesForChalets { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TripFeedback> TripFeedbacks { get; set; }

    public virtual DbSet<UserGuest> UserGuests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SID=xe)));User Id=C##MVC_REH;Password=REH123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##MVC_REH")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<AdventureTrip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008394");

            entity.ToTable("ADVENTURE_TRIP");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.CompanyOwnerId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("COMPANY_OWNER_ID");
            entity.Property(e => e.DateFrom)
                .HasColumnType("DATE")
                .HasColumnName("DATE_FROM");
            entity.Property(e => e.DateTo)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TO");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.JoinDate)
                .HasColumnType("DATE")
                .HasColumnName("JOIN_DATE");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.NumberOfGuests)
                .HasColumnType("NUMBER")
                .HasColumnName("NUMBER_OF_GUESTS");
            entity.Property(e => e.Price)
                .HasColumnType("FLOAT")
                .HasColumnName("PRICE");
            entity.Property(e => e.ProofOfCompanyOwner)
                .IsUnicode(false)
                .HasColumnName("PROOF_OF_COMPANY_OWNER");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.CompanyOwner).WithMany(p => p.AdventureTrips)
                .HasForeignKey(d => d.CompanyOwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008395");
        });

        modelBuilder.Entity<BookChalet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008467");

            entity.ToTable("BOOK_CHALET");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.ChaletId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CHALET_ID");
            entity.Property(e => e.DateId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("DATE_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Chalet).WithMany(p => p.BookChalets)
                .HasForeignKey(d => d.ChaletId)
                .HasConstraintName("SYS_C008468");

            entity.HasOne(d => d.Date).WithMany(p => p.BookChalets)
                .HasForeignKey(d => d.DateId)
                .HasConstraintName("FK_BOOK_CHALETS_DATE");

            entity.HasOne(d => d.User).WithMany(p => p.BookChalets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("SYS_C008469");
        });

        modelBuilder.Entity<BookTrip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008399");

            entity.ToTable("BOOK_TRIP");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.AdventureTripId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ADVENTURE_TRIP_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.AdventureTrip).WithMany(p => p.BookTrips)
                .HasForeignKey(d => d.AdventureTripId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008402");

            entity.HasOne(d => d.User).WithMany(p => p.BookTrips)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008400");
        });

        modelBuilder.Entity<Chalet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008391");

            entity.ToTable("CHALET");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.JoinDate)
                .HasColumnType("DATE")
                .HasColumnName("JOIN_DATE");
            entity.Property(e => e.LacationLatitude)
                .IsUnicode(false)
                .HasColumnName("LACATION_LATITUDE");
            entity.Property(e => e.LacationLongitude)
                .IsUnicode(false)
                .HasColumnName("LACATION_LONGITUDE");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.NumberOfGuests)
                .HasColumnType("NUMBER")
                .HasColumnName("NUMBER_OF_GUESTS");
            entity.Property(e => e.OwnerId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("OWNER_ID");
            entity.Property(e => e.Price)
                .HasColumnType("FLOAT")
                .HasColumnName("PRICE");
            entity.Property(e => e.ProofOfOwner)
                .IsUnicode(false)
                .HasColumnName("PROOF_OF_OWNER");
            entity.Property(e => e.Rating)
                .HasColumnType("FLOAT")
                .HasColumnName("RATING");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Owner).WithMany(p => p.Chalets)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008392");
        });

        modelBuilder.Entity<ChaletFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008478");

            entity.ToTable("CHALET_FEEDBACK");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.ChaletId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CHALET_ID");
            entity.Property(e => e.Text)
                .IsUnicode(false)
                .HasColumnName("TEXT");
            entity.Property(e => e.Rate)
                .HasColumnType("FLOAT")
                .HasColumnName("RATE");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Chalet).WithMany(p => p.ChaletFeedbacks)
                .HasForeignKey(d => d.ChaletId)
                .HasConstraintName("SYS_C008479");
            entity.HasOne(d => d.User).WithMany(p => p.ChaletFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_CHALET_FEEDBACK_USER");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008397");

            entity.ToTable("CONTACT_US");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUBJECT");
            entity.Property(e => e.Text)
                .IsUnicode(false)
                .HasColumnName("TEXT");
        });

        modelBuilder.Entity<ListOfDatesForChalet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008411");

            entity.ToTable("LIST_OF_DATES_FOR_CHALETS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.ChaletId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CHALET_ID");
            entity.Property(e => e.DateFrom)
                .HasColumnType("DATE")
                .HasColumnName("DATE_FROM");
            entity.Property(e => e.DateTo)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TO");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Chalet).WithMany(p => p.ListOfDatesForChalets)
                .HasForeignKey(d => d.ChaletId)
                .HasConstraintName("SYS_C008412");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008386");

            entity.ToTable("ROLE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<TripFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008481");

            entity.ToTable("TRIP_FEEDBACK");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Text)
                .IsUnicode(false)
                .HasColumnName("TEXT");
            entity.Property(e => e.TripId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TRIP_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripFeedbacks)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("SYS_C008482");
            entity.HasOne(d => d.User).WithMany(p => p.TripFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TRIP_FEEDBACK_USER");
        });

        modelBuilder.Entity<UserGuest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008388");

            entity.ToTable("USER_GUEST");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.ConfirmPassword)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("CONFIRM_PASSWORD");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.JoinDate)
                .HasColumnType("DATE")
                .HasColumnName("JOIN_DATE");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.RoleId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROLE_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserGuests)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008389");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
