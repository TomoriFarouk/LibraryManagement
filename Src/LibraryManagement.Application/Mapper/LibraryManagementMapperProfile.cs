
using System;
using AutoMapper;
using LibraryManagement.Application.Response;
using LibraryManagement.Core.Entities;
using static LibraryManagement.Application.Command.BookCommand;
using static LibraryManagement.Application.Command.BorrowingRecordCommand;
using static LibraryManagement.Application.Command.PatronCommand;

namespace LibraryManagement.Application.Mapper
{
	public class LibraryManagementMapperProfile:Profile
	{
		public LibraryManagementMapperProfile()
		{
            CreateMap<Books, BookResponse>().ReverseMap();
            CreateMap<Books, CreateBookCommand>().ReverseMap();
            CreateMap<Books, EditBookCommand>().ReverseMap();
            CreateMap<Patron, PatronResponse>().ReverseMap();
            CreateMap<Patron, CreatePatronCommand>().ReverseMap();
            CreateMap<Patron, EditPatronCommand>().ReverseMap();
            CreateMap<BorrowingRecord, BorrowingRecordResponse>().ReverseMap();
            CreateMap<BorrowingRecord, CreateBorrowingRecordCommand>().ReverseMap();
            CreateMap<BorrowingRecord, ReturnBookRecordCommand>().ReverseMap();
            CreateMap<BorrowingRecord, EditBorrowingRecordCommand>().ReverseMap();
        }
	}
}

