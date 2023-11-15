using AutoMapper;
using DocumentService.Word.Models;
using DocumentServiceWebAPI.Models;

namespace DocumentServiceWebAPI.Helpers;

public class AutoMappingProfile : Profile
{
    public AutoMappingProfile()
    {
        this.CreateMap<WordContentDataRequestDTO, ContentData>();
    }
}
