using System;
using LibraryManagement.Application.Response;
using MediatR;

namespace LibraryManagement.Application.Command
{
	public class PatronCommand
	{
        public class CreatePatronCommand : IRequest<PatronResponse>
        {

            public string Name { get; set; }


            public string ContactInformation { get; set; }

            public string UserId { get; set; }

            public DateTime CreatedDate { get; set; }

            public CreatePatronCommand()
            {
                this.CreatedDate = DateTime.Now;
            }
        }

        public class DeletePatronCommand : IRequest<String>
        {
            public int Id { get; set; }

            public DeletePatronCommand(int Id)
            {
                this.Id = Id;
            }
        }

        public class EditPatronCommand : IRequest<PatronResponse>
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public string ContactInformation { get; set; }

            public string UserId { get; set; }

            public DateTime ModifiedDate { get; set; }


            public EditPatronCommand()
            {
                this.ModifiedDate = DateTime.Now;
            }

        }
    }
}

