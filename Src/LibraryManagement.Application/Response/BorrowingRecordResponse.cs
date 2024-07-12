using System;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Application.Response
{
	public class BorrowingRecordResponse
	{
        public int Id { get; set; }

        public int BookId { get; set; }

       
        public int PatronId { get; set; }

      
        public string UserId { get; set; }

        public byte[] RowVersion { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}

