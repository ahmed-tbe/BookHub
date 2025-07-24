using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookHub.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Category name")]
        [MaxLength(30)]
        public string Name { get; set; }

        [DisplayName("Display order")]
        [Range(1, 100, ErrorMessage = "Order must be between 1-100")]
        public int DisplayOrder { get; set; } //if there's multiple categories, which one display 1st
    }
}
