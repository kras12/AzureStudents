﻿using AutoMapper;
using AzureStudents.Data.Entities;
using AzureStudents.Shared.Dto.Student;

namespace AzureStudents.Api.Mapping;

/// <summary>
/// An auto mapper profile that contains mappings for converting entities to/from DTOs. 
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public AutoMapperProfile()
    {
        CreateMap<Student, StudentDto>().ReverseMap();
        CreateMap<Student, CreateStudentDto>().ReverseMap();
        CreateMap<Student, UpdateStudentDto>().ReverseMap();
    }
}
