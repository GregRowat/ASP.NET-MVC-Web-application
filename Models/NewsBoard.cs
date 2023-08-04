using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Lab4.Models
{
    public class NewsBoard
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registration Number")]
        [Required]
        public String Id { get; set; }

        [Required]
        [StringLength(50), MinLength(3)]
        public String Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public Decimal Fee { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

        public ICollection<News> News { get; set; }
    }
}
