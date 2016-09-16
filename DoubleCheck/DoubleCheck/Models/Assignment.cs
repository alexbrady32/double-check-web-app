namespace DoubleCheck.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Assignment
    {
        [Key]
        public int A_Id { get; set; }

        public int U_Id { get; set; }

        public int C_Id { get; set; }

        public int T_Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        public int Status { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Due_Date { get; set; }

        public int TTC { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Description { get; set; }

        public virtual Asgmt_Type Asgmt_Type { get; set; }

        public virtual Class Class { get; set; }

        public virtual User User { get; set; }
    }
}
