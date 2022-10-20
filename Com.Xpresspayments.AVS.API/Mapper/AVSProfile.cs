using AutoMapper;
using Com.Xpresspayments.AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.API.Mapper
{
    public class AVSProfile : Profile
    {
        public AVSProfile()
        {
            CreateMap<FidelityResponseVM, GenericProviderResponse>();
            CreateMap<GTNameEnquiryDetails, GenericProviderResponse>();
            CreateMap<NESingleResponse, GenericProviderResponse>();
            CreateMap<UBANameEnquiryOthersResponse, GenericProviderResponse>().ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.CustomerName)).ForMember(dest => dest.ResponseCode, opt => opt.MapFrom(src => src.StatusCode));
            CreateMap<UBANameEnquiryResponse, GenericProviderResponse>().ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.CustomerName)).ForMember(dest => dest.ResponseCode, opt => opt.MapFrom(src => src.StatusCode)); ;
        }
        
    }
}
