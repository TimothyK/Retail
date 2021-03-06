﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail.Data.SqlDb.EfModels.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int CustomerId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string Province { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [StringLength(20)]
        public string PostalCode { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        [Required]
        public bool? Active { get; set; }
        public Guid MembershipNumber { get; set; }
        public double Discount { get; set; }

        [InverseProperty(nameof(Order.Customer))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}