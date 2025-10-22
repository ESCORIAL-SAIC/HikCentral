using HikCentral.Utils;
using Newtonsoft.Json;

namespace HikCentral.Controllers.AccessLevelAndAccessGroup;

public class InformationListOfPersonController
{
    protected InformationListOfPersonController() { }
    public static async Task<Models.AccessLevelAndAccessGroup.InformationListOfPerson> Get(int pageNo, int pageSize, int type, string privilegeGroupId)
    {
        if (Configuration.Setting.Endpoint.GetPersonsRelatedToAccessLevel is null || Configuration.Setting.Endpoint.GetPersonsRelatedToAccessLevel.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.GetPersonsRelatedToAccessLevel)} es nulo");
        var apiPath = Configuration.Setting.Endpoint.GetPersonsRelatedToAccessLevel.Path;
        var body = new
        {
            pageNo,
            pageSize,
            type,
            privilegeGroupId
        };
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        var accessLevelList = JsonConvert.DeserializeObject<Models.AccessLevelAndAccessGroup.InformationListOfPerson>(await response.Content.ReadAsStringAsync());
        return accessLevelList is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : accessLevelList;
    }
}