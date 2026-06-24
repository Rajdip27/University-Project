using Mapster;
using UniversityProject.Application.ViewModel;
using UniversityProject.Core.Entities;

namespace UniversityProject.Application.Mappings;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;
    }
}
