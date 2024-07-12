using System;
using LibraryManagement.Application.Response;
using MediatR;

namespace LibraryManagement.Application.Command
{
	public class BorrowingRecordCommand
	{
        public class CreateBorrowingRecordCommand : IRequest<BorrowingRecordResponse>
        {

            public int BookId { get; set; }


            public int PatronId { get; set; }


            public string UserId { get; set; }

            public bool IsReturned { get; set; }

            public DateTime BorrowDate { get; set; }

            public CreateBorrowingRecordCommand()
            {
                this.BorrowDate = DateTime.Now;
                this.IsReturned = false;
            }
        }

        public class ReturnBookRecordCommand : IRequest<BorrowingRecordResponse>
        {

            public int BookId { get; set; }


            public int PatronId { get; set; }

            public bool IsReturned { get; set; }

            public string UserId { get; set; }

            public DateTime ReturnedDate { get; set; }


            public ReturnBookRecordCommand()
            {
                this.ReturnedDate = DateTime.Now;
                this.IsReturned = true;
            }
        }

        public class DeleteBorrowingRecordCommand : IRequest<String>
        {
            public int Id { get; set; }

            public DeleteBorrowingRecordCommand(int Id)
            {
                this.Id = Id;
            }
        }

        public class EditBorrowingRecordCommand : IRequest<BorrowingRecordResponse>
        {
            public int Id { get; set; }
            public string Name { get; set; }


            public string ContactInformation { get; set; }

            public DateTime ModifiedDate { get; set; }

            public DateTime LastModified { get; set; }

            public byte[] RowVersion { get; set; }

            public EditBorrowingRecordCommand()
            {
                this.ModifiedDate = DateTime.Now;
            }

        }
    }
}

