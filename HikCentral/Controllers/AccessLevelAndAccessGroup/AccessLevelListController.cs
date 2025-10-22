using HikCentral.Utils;
using Newtonsoft.Json;

namespace HikCentral.Controllers.AccessLevelAndAccessGroup;

public class AccessLevelListController
{
    protected AccessLevelListController() { }
    public static async Task<Models.AccessLevelAndAccessGroup.AccessLevelList> Get(int pageNumber, int pageSize, int type)
    {
        if (Configuration.Setting.Endpoint.GetAccessLevelList is null || Configuration.Setting.Endpoint.GetAccessLevelList.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.GetAccessLevelList)} es nulo.");
        var apiPath = Configuration.Setting.Endpoint.GetAccessLevelList.Path;
        var body = new
        {
            pageNo = pageNumber,
            pageSize,
            type
        };
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        var accessLevelList = JsonConvert.DeserializeObject<Models.AccessLevelAndAccessGroup.AccessLevelList>(await response.Content.ReadAsStringAsync());
        return accessLevelList is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : accessLevelList;
    }
}