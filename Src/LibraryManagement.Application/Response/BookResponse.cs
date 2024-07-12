using System;
namespace LibraryManagement.Application.Response
{
	public class BookResponse
	{
        public int Id { get; set; }

        public string Title { get; set; }


        public string Author { get; set; }


        public int PublicationYear { get; set; }

        public int count { get; set; }

        public string ISBN { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte[] RowVersion { get; set; }
    }
}

