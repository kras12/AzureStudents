using AutoMapper;
using AzureStudents.Blazor.ViewModels;
using AzureStudents.Shared.Dto.Student;

namespace AzureStudents.Blazor.Mapping;

/// <summary>
/// An auto mapper profile that contains mappings for converting DTOs to/from ViewModels.
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public AutoMapperProfile()
    {
        CreateMap<StudentViewModel, StudentViewModel>();        
        CreateMap<StudentViewModel, CreateStudentDto>();
        CreateMap<StudentViewModel, UpdateStudentDto>();
        CreateMap<StudentDto, StudentViewModel>();
    }
}
