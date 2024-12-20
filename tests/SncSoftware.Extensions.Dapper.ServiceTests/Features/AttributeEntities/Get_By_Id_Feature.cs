using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Get_By_Id_Feature
{
    [Scenario]
    public async Task Get_By_Id_Returns_Entity_By_Id()
    {
        await Runner.RunScenarioAsync(
            given => Entity_exists_in_database(),
            when => Get_By_Id_is_requested(),
            then => Retrieved_entity_should_match_source());
    }
}