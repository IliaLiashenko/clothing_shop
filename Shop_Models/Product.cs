﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Models
{
    public class Product
    {
        public Product()
        {
            DisplayOrder = 1;
        }
        public int Id { get; set; }

        [DisplayName("Name")]
        public string ProductName { get; set; }
        
        [DisplayName("Description")]
        public string ProductDescription { get; set; }
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        [ValidateNever]
        [NotMapped]
        public virtual ICollection<ProductSize> ProductSizes { get; set; }
        [Display(Name = "Color")]
        [ValidateNever]
        public int ColorsId { get; set; }
        [ForeignKey("ColorsId")]
        [ValidateNever]
        public virtual Colors Colors { get; set; }


        [ValidateNever]
        public string Image { get; set; }

        [ValidateNever]
        [DisplayName("Display order")]
        
        [Range(1, int.MaxValue,ErrorMessage = "Display order for product must be greater than 0")]
        
        public int DisplayOrder {  get; set; }
        [Display(Name = "Category type")]
        
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public virtual Category Category { get; set; }

        [NotMapped]
        [ValidateNever]
        public Size Size { get; set; }

        [Display(Name = "Gender")]
        public int GenderId { get; set; }
        [ForeignKey("GenderId")]
        [ValidateNever]
        public virtual Gender Gender { get; set; }

        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        [ValidateNever]
        public virtual Brand Brand { get; set; }

        [Display(Name = "Style")]
        public int StyleId { get; set; }
        [ForeignKey("StyleId")]
        [ValidateNever]
        public virtual Style Style { get; set; }

    }
}
