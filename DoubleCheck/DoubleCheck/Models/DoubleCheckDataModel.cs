/*namespace DoubleCheck.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Runtime.Remoting.Contexts;
    using System.Web.Configuration;

    public partial class DoubleCheckDataModel : WebContext
    {
        public DoubleCheckDataModel()
            : base("name=DoubleCheckDataModel")
        {
        }

        public virtual DbSet<Asgmt_Type> Asgmt_Type { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asgmt_Type>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Asgmt_Type>()
                .HasMany(e => e.Assignments)
                .WithRequired(e => e.Asgmt_Type)
                .HasForeignKey(e => e.T_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.Start_Time)
                .HasPrecision(0);

            modelBuilder.Entity<Class>()
                .Property(e => e.End_Time)
                .HasPrecision(0);

            modelBuilder.Entity<Class>()
                .Property(e => e.Days)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.Building)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Assignments)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.firstName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.lastName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.phone_num)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Assignments)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.U_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Classes)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.U_Id)
                .WillCascadeOnDelete(false);
        }
    }
}*/
