using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Profiles;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>();
            //CreateMap method should be called in the contructor of MappingProfiles

            CreateMap<Activity, ActivityDto>()
                .ForMember(
                    dest => dest.HostName,
                    opt => opt.MapFrom(activity => activity.Attendees.FirstOrDefault(aa => aa.IsHost).AppUser.UserName));

            CreateMap<ActivityAttendee, UserProfile>()
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.MapFrom(aa => aa.AppUser.DisplayName))
                .ForMember(
                    dest => dest.Bio,
                    opt => opt.MapFrom(aa => aa.AppUser.Bio))
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(aa => aa.AppUser.UserName));
        }
    }
}