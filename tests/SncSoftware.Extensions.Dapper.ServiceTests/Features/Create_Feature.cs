using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features;

public partial class Create_Feature
{
    [Scenario]
    public async Task Create_persists_entity_to_database()
    {
        await Runner.RunScenarioAsync(
            when => Create_is_requested_and_saved(),
            then => Entity_is_created_in_the_database());
    }
}