using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TECH.SharedKernel;

namespace TECH.Data.DatabaseEntity
{
    [Table("products")]
    public class Products : DomainEntity<int>
    {
        [Column(TypeName = "nvarchar(250)")]
        public string? name { get; set; }
        public int? category_id { get; set; }
        [ForeignKey("category_id")]
        public Category? Category { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal? price_sell { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal? price_reduced { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal? price_import { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string? images { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? description { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? specifications { get; set; }
        public int? color { get; set; }
        //public int? color { get; set; }
        public int? quantity { get; set; }
        public int? quantitysell { get; set; }        
        public int? status { get; set; }
        public int? ishidden { get; set; }
        
    }
}
