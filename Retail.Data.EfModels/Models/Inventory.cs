﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail.Data.SqlDb.EfModels.Models
{
    [Table("Inventory")]
    public partial class Inventory
    {
        [Key]
        public int StoreId { get; set; }
        [Key]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}