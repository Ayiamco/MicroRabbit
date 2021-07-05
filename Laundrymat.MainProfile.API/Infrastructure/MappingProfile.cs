using AutoMapper;
using Laundromat.MainProfile.API.Enitities;
using Laundromat.MainProfile.API.RequestModels.CommandRequests;
using Laundromat.SharedKernel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddLaundryRequestModel, NewLaundry>().ReverseMap();
            CreateMap<UpdateLaundryRequestModel, Laundry>()
                .ForMember(x => x.UpdatedAt, y => y.MapFrom(d => DateTime.Now))
                .ForMember(x => x.CreatedAt, y => y.Ignore());

            CreateMap<Laundry, Laundry>()
              .ForMember(x => x.UpdatedAt, y => y.MapFrom(d => DateTime.Now))
              .ForMember(x => x.CreatedAt, y => y.Ignore())
              .ForMember(x => x.Id, y => y.Ignore());

            CreateMap<AddLaundryRequestModel, Laundry>()
               .ForMember(x => x.UpdatedAt, y => y.MapFrom(d => DateTime.Now))
               .ForMember(x => x.CreatedAt, y => y.MapFrom(d=> DateTime.Now));

            //CreateMap<NewCustomerDto, Customer>()
            //    .ForMember(x => x.CreatedAt, y => y.MapFrom(x => DateTime.Now))
            //    .ForMember(x => x.UpdatedAt, y => y.MapFrom(x => DateTime.Now));
        }
    }
}
