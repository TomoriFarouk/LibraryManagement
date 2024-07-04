using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryManagement.Core.Entities.Base;
using LibraryManagement.Core.Entities.Identity;

namespace LibraryManagement.Core.Entities
{
	public class Books:BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }

        [Range(1450, int.MaxValue, ErrorMessage = "Please enter a valid year.")]
        public int PublicationYear { get; set; }
        [Required]
        public int count { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [RegularExpression(@"\d{3}-\d{10}", ErrorMessage = "ISBN must be in the format xxx-xxxxxxxxxx.")]
        public string ISBN { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public ApplicationUser applicationUser { get; set; }

        public ICollection<BorrowingRecord>BorrowingRecords { get; set; }
        public ICollection<Patron> patrons { get; set; }
    }
}

