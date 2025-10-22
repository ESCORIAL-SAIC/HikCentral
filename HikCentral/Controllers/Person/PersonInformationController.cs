using HikCentral.Utils;
using Newtonsoft.Json;

namespace HikCentral.Controllers.Person;

public class PersonInformationController
{
    protected PersonInformationController() { }
    public static async Task<Models.Person.PersonInformation> Get(int pageNumber, int pageSize)
    {
        if (Configuration.Setting.Endpoint.PersonGet is null || Configuration.Setting.Endpoint.PersonGet.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.PersonGet)} es nulo.");
        var apiPath = Configuration.Setting.Endpoint.PersonGet.Path;
        var body = new
        {
            pageNo = pageNumber,
            pageSize
        };
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");

        var personInformation = JsonConvert.DeserializeObject<Models.Person.PersonInformation>(await response.Content.ReadAsStringAsync());
        return personInformation is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : personInformation;
    }
}