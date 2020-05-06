﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retail.Data.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderLineItems = new HashSet<OrderLineItem>();
        }

        [Key]
        public int OrderId { get; set; }
        public int StoreId { get; set; }
        public int? CustomerId { get; set; }
        [StringLength(100)]
        public string ShippingAddress { get; set; }
        [StringLength(50)]
        public string ShippingCity { get; set; }
        [StringLength(50)]
        public string ShippingProvince { get; set; }
        [StringLength(50)]
        public string ShippingCountry { get; set; }
        [StringLength(20)]
        public string ShippingPostalCode { get; set; }
        [StringLength(20)]
        public string CustomerPhoneNumber { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Orders")]
        public virtual Customer Customer { get; set; }
        [ForeignKey(nameof(StoreId))]
        [InverseProperty("Orders")]
        public virtual Store Store { get; set; }
        [InverseProperty(nameof(OrderLineItem.Order))]
        public virtual ICollection<OrderLineItem> OrderLineItems { get; set; }
    }
}