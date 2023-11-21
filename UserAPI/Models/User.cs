using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserAPI.Validation;

namespace UserAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [EmailAddress]
        [StringLength(256, ErrorMessage = "The email address cannot exceed 256 characters.")]
        public string? Email { get; set; }

        [PhoneNumber]
        public string? Phone { get; set; }

        [Password]
        public required string Password { get; set; }
    }
}
