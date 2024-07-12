using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryManagement.Core.Entities.Base;
using LibraryManagement.Core.Entities.Identity;

namespace LibraryManagement.Core.Entities
{
	public class Patron:BaseEntity
	{
		public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string ContactInformation { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public ApplicationUser applicationUser { get; set; }


        public ICollection<BorrowingRecord> BorrowingRecords { get; set; }

    }
}

