using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Update_Feature
{
    [Scenario]
    public async Task Update_persists_update_to_database()
    {
        await Runner.RunScenarioAsync(
            given => Entity_exists_in_the_database(),
            when => Update_is_requested_and_saved(),
            then => Updated_entity_is_saved_in_database());
    }
}