using HikCentral.Utils;
using HikCentral.Controllers.AccessLevelAndAccessGroup;
using Newtonsoft.Json;

namespace HikCentral.Controllers.Person;

public class AccessLevelController
{
    protected AccessLevelController() { }
    public static async Task<Models.Person.AccessLevel?> ReApply()
    {
        if (Configuration.Setting.Endpoint.ReApplyAccessSettings is null || Configuration.Setting.Endpoint.ReApplyAccessSettings.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.ReApplyAccessSettings)} es nulo.");
        var apiPath = Configuration.Setting.Endpoint.ReApplyAccessSettings.Path;
        var body = new
        {
            InmediateDownload = 0
        };
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        var accessLevel = JsonConvert.DeserializeObject<Models.Person.AccessLevel>(await response.Content.ReadAsStringAsync());
        return accessLevel is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : accessLevel;
    }
    public static async Task DisablePersonAccess(Models.Person.PersonInformation.List person)
    {
        var response = await AccessLevelListController.Get(1, 500, 1) ?? throw new AccessLevelAndAccessGroupNotRetrievedException("No se pudieron obtener los grupos de acceso.");
        foreach (var device in response.data.list)
            await AccessLevelAndAccessGroup.EditPersonController.Delete(person.personId, device.privilegeGroupId);
        var editResponse = await EditPersonController.Disable(person);
        if (editResponse == null || editResponse.code != "0")
            throw new InvalidOperationException("No se pudo mover a la persona al grupo \"Inactivos\".");
        var reApplyResponse = await ReApply();
        if (reApplyResponse == null || reApplyResponse.code != "0")
            throw new AccessLevelReApplyException($"Error al aplicar los permisos en dispositivos de acceso. code {reApplyResponse?.code ?? "N/A"}, msg {reApplyResponse?.msg ?? "N/A"}");
    }
}