using HikCentral.Utils;
using Newtonsoft.Json;

namespace HikCentral.Controllers.AccessLevelAndAccessGroup;
public class EditPersonController
{
    protected EditPersonController() { }
    public static async Task<Models.AccessLevelAndAccessGroup.EditPerson?> Delete(string personId, string privilegeGroupId)
    {
        if (Configuration.Setting.Endpoint.DeletePersonFromAccessLevel is null || Configuration.Setting.Endpoint.DeletePersonFromAccessLevel.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.DeletePersonFromAccessLevel)} es nulo.");
        var apiPath = Configuration.Setting.Endpoint.DeletePersonFromAccessLevel.Path;
        var body = new
        {
            privilegeGroupId,
            type = 1,
            list = new[] {
                new {
                    id = $"{personId}"
                }
            }
        };
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        var editPerson = JsonConvert.DeserializeObject<Models.AccessLevelAndAccessGroup.EditPerson>(await response.Content.ReadAsStringAsync());
        return editPerson is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : editPerson;
    }
    public static async Task<Models.AccessLevelAndAccessGroup.EditPerson?> Add(string personId, string privilegeGroupId)
    {
        if (Configuration.Setting.Endpoint.AddPersonToAccessLevel is null || Configuration.Setting.Endpoint.AddPersonToAccessLevel.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.AddPersonToAccessLevel)} es nulo.");
        var apiPath = Configuration.Setting.Endpoint.AddPersonToAccessLevel.Path;
        var body = new
        {
            privilegeGroupId,
            type = 1,
            list = new[] {
                new {
                    id = $"{personId}"
                }
            }
        };
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        var editPerson = JsonConvert.DeserializeObject<Models.AccessLevelAndAccessGroup.EditPerson>(await response.Content.ReadAsStringAsync());
        return editPerson is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : editPerson;
    }
}