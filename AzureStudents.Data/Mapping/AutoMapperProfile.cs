using AutoMapper;
using AzureStudents.Data.Entities;
using AzureStudents.Shared.Dto.Student;

namespace AzureStudents.Data.Mapping;

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
        CreateMap<Student, StudentDto>()
            .ReverseMap();

        CreateMap<CreateStudentDto, Student>();
        CreateMap<EditStudentDto, Student>();
    }
}
