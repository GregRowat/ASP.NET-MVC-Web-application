using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class News
    {
        public int Id { get; set; }
        public string NewsBoardId { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public NewsBoard newsboard { get; set; }
    }
}
