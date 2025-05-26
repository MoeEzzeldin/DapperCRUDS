using AutoMapper;
using CRUDWithDapperORMD8.Models;
using CRUDWithDapperORMD8.Models.DTOs;
namespace CRUDWithDapperORMD8.Mappings
{
    public class BookMapper : Profile
    {
        public BookMapper() 
        {
            CreateMap<Book, BookDTO>();

            CreateMap<BookDTO, Book>();

            /// <summary>
            /// I also learned that some advanced mapping techniques could look like:
            /// CreateMap<Book, BookDTO>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title + " by " + src.Author)); for custom values
            /// CreateMap<BookDTO, Book>().ForMember((dest => dest.Price, opt => opt.Condition(src => src.Price > 0)); Conditionally map properties

        }
    }
}
