using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Models
{
	public class PhotoGallery
	{
		[Key]
		public int Id { get; set; }
		public string Url { get; set; }
		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public virtual Product Product { get; set; }
	}
}
