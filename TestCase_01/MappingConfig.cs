using AutoMapper;
using System;
using System.Linq;
using TestCase_01_DataAccess.Entities;
using TestCase_01_DTO;

namespace TestCase_01
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            
            CreateMap<TestCase, TestCaseDTO>()
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps.Select(s => s.Step).ToList()));

            
            CreateMap<TestCaseDTO, TestCase>()
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps.Select(stepText => new TestCaseStep { Step = stepText }).ToList()));
           
                


       
            CreateMap<TestCase, TestCaseResponseDTO>()
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps.Select(s => s.Step).ToList()));

          
            CreateMap<TestCaseResponseDTO, TestCase>()
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps.Select(stepText => new TestCaseStep { Step = stepText }).ToList()));
        }
    }
}