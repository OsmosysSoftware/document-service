using AutoMapper;
using DocumentService.Word.Models;
using DocumentService.API.Models;

namespace DocumentService.API.Helpers;

public class AutoMappingProfile : Profile
{
    public AutoMappingProfile()
    {
        this.CreateMap<WordContentDataRequestDTO, ContentData>();
    }
}
