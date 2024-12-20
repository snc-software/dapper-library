using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace SncSoftware.Extensions.Dapper.ServiceTests.Features.AttributeEntities;

public partial class Delete_By_Id_Feature
{
    [Scenario]
    public async Task Delete_By_Id_Deletes_The_Entity()
    {
        await Runner.RunScenarioAsync(
            given => Entity_exists_in_database(),
            when => Delete_is_requested_and_saved(),
            then => Entity_is_deleted());
    }
}