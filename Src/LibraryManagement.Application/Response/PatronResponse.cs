using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Application.Response
{
	public class PatronResponse
	{
        public int Id { get; set; }

       
        public string Name { get; set; }

       
        public string ContactInformation { get; set; }

        public byte[] RowVersion { get; set; }

    }
  }


