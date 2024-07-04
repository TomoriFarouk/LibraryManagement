using System;
using System.ComponentModel.DataAnnotations;
using LibraryManagement.Application.Response;
using MediatR;

namespace LibraryManagement.Application.Command
{
	public class BookCommand
	{
        public class CreateBookCommand : IRequest<BookResponse>
        {
           
            public string Title { get; set; }

          
            public string Author { get; set; }


            public int PublicationYear { get; set; }
      
            public int count { get; set; }

            public string ISBN { get; set; }

            public DateTime CreatedDate { get; set; }

            public string UserId { get; set; }

            public CreateBookCommand()
            {
                this.CreatedDate = DateTime.Now;
            }
        }

        public class DeleteBookCommand : IRequest<String>
        {
            public int Id { get; set; }

            public DeleteBookCommand(int Id)
            {
                this.Id = Id;
            }
        }

        public class EditBookCommand : IRequest<BookResponse>
        {
            public int Id { get; set; }
            public string Title { get; set; }


            public string Author { get; set; }


            public int PublicationYear { get; set; }

            public int count { get; set; }

            public string ISBN { get; set; }

            public string UserId { get; set; }

            public DateTime ModifiedDate { get; set; }

            public EditBookCommand()
            {
                this.ModifiedDate = DateTime.Now;
            }

        }
    }
}

