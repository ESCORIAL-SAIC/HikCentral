using HikCentral.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HikCentral.Controllers.Person;

public class EditPersonController
{
    protected EditPersonController() { }
    public static async Task<Models.Person.EditPerson> EditPerson(Models.Person.PersonInformation.List person)
    {
        if (Configuration.Setting.Endpoint.PersonUpdate is null || Configuration.Setting.Endpoint.PersonUpdate.Path is null)
            throw new EndpointNullException($"El endpoint {nameof(Configuration.Setting.Endpoint.PersonUpdate)} es nulo.");
        var apiPath = Configuration.Setting.Endpoint.PersonUpdate.Path;
        var body = JObject.FromObject(person);
        var requestMessage = Fun.GenerateHttpRequestMessage(body);
        var httpClient = Fun.GenerateHttpClient(apiPath);
        var response = await httpClient.PostAsync($"{Configuration.Setting.ServerAddress}{apiPath}", requestMessage.Content);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        var editPerson = JsonConvert.DeserializeObject<Models.Person.EditPerson>(await response.Content.ReadAsStringAsync());
        return editPerson is null
            ? throw new InvalidOperationException("The response content could not be deserialized into a RootObject.")
            : editPerson;
    }

    public static async Task<Models.Person.PersonInformation.List> FindPersonByDni(string dni)
    {
        int pageNo = 1;
        int maxPages = 100;
        while (pageNo <= maxPages)
        {
            var response = await Person.PersonInformationController.Get(pageNo, 500);
            if (response == null || response.data.pageNo != pageNo)
                throw new PersonInformationNotRetrievedException("No se pudo obtener información de la persona.");
            var person = Array.Find(response.data.list, e => e.cards != null && e.cards[0].cardNo == dni);
            if (person != null)
                return person;
            pageNo++;
        }
        throw new PersonNotFoundException("Persona no encontrada.");
    }
}