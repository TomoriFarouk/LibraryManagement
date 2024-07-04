using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryManagement.Core.Entities.Identity;

namespace LibraryManagement.Core.Entities
{
	public class BorrowingRecord
	{
        public int Id { get; set; }
        [Required]
        public bool IsReturned { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        public Books Book { get; set; }

        [Required]
        [ForeignKey("Patron")]
        public int PatronId { get; set; }

        public Patron patron { get; set; }


        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public ApplicationUser applicationUser { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

    }
}

