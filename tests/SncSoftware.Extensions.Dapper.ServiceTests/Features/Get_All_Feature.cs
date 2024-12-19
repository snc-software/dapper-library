using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features;

public partial class Get_All_Feature
{
    [Scenario(Skip = "Conflicts with other fixtures")]
    public async Task Get_all_returns_all_entities()
    {
        await Runner.RunScenarioAsync(
            given => Entities_exists_in_database(),
            when => Get_all_is_requested(),
            then => Retrieved_entities_match_source());
    }
}