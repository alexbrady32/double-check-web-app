namespace DoubleCheck.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Class")]
    public partial class Class
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Class()
        {
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        public int C_Id { get; set; }

        public int U_Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        public TimeSpan Start_Time { get; set; }

        public TimeSpan End_Time { get; set; }

        [Required]
        [StringLength(7)]
        public string Days { get; set; }

        [StringLength(20)]
        public string Building { get; set; }

        public int? Room_Num { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Assignment> Assignments { get; set; }

        public virtual User User { get; set; }
    }
}
