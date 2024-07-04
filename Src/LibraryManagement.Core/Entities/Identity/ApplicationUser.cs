using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Core.Entities.Identity
{
	public class ApplicationUser:IdentityUser
	{
        public string? FullName { get; set; }
        public ICollection<Books> book { get; set; }
        public ICollection<BorrowingRecord> borrowingRecords { get; set; }
        public ICollection<Patron> patrons { get; set; }
    }
}

