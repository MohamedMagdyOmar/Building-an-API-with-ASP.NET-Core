using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            // yo can do this also for all properties
            this.CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));

            this.CreateMap<CampModel, Camp>();

            this.CreateMap<Talk, TalkModel>();

            this.CreateMap<Speaker, SpeakerModel>();
        }
    }
}
