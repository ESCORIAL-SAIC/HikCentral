using HikCentral.Utils;

namespace HikCentral.Acceso;
class Program
{
    protected Program() { }
    static async Task Main(string[] args)
    {
        Configuration.Load();
        string[]? accessArray;
        if (args.Length == 0)
        {
            Console.WriteLine("No se ingresaron los parámetros correspondientes");
            Console.WriteLine("Presione una tecla para finalizar");
            return;
        }
        var dni = args[0];
        var mailToNotify = args[1];
        var mailService = Fun.InitializeMailService(mailToNotify);
        var recipients = Configuration.Setting.Recipients;
        try
        {
            if (args.Length > 2)
                accessArray = args[2].Split(';');
            else
                accessArray = null;
            var person = await GetPersonByDni(dni);
            var personId = person.personId;
            if (personId == null)
            {
                Console.WriteLine("Person not found.");
                return;
            }
            var isModified = await UpdateAccessLevels(person, accessArray);
            if (isModified)
            {
                Console.WriteLine($"Aplicando cambios.");
                await ReapplyAccessLevels();
            }
            else
            {
                Console.WriteLine($"No hay cambios para aplicar.");
            }
            await Fun.NotifySuccess(mailService, "Se modificaron correctamente los accesos de la persona.", recipients, person.personName, dni);
            Console.WriteLine("Proceso finalizado. Presione una tecla para finalizar.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Fun.HandleException(ex, "Error en modificación de permisos de acceso.", dni, mailService, recipients);
        }
    }

    private static async Task<Models.Person.PersonInformation.List> GetPersonByDni(string dni)
    {
        var person = await Controllers.Person.EditPersonController.FindPersonByDni(dni);
        return person;
    }

    private static async Task<bool> UpdateAccessLevels(Models.Person.PersonInformation.List person, string[]? accessArray)
    {
        var personId = person.personId;
        var personName = person.personName;
        var accessGroup = await Controllers.AccessLevelAndAccessGroup.AccessLevelListController.Get(1, 500, 1);
        var isModified = false;
        foreach (var access in accessGroup.data.list)
        {
            var privilegeGroupId = access.privilegeGroupId;
            var personRelated = await GetPersonRelatedToAccess(privilegeGroupId, personId);
            if (ShouldRemovePersonFromAccess(accessArray, privilegeGroupId, personRelated))
            {
                Console.WriteLine($"Eliminando a {personName} de {access.privilegeGroupName} ");
                await RemovePersonFromAccess(personId, privilegeGroupId);
                isModified = true;
            }
            else if (ShouldAddPersonToAccess(accessArray, privilegeGroupId, personRelated))
            {
                Console.WriteLine($"Añadiendo a {personName} a {access.privilegeGroupName}");
                await AddPersonToAccess(personId, privilegeGroupId);
                isModified = true;
            }
        }
        return isModified;
    }

    private static async Task<dynamic?> GetPersonRelatedToAccess(string privilegeGroupId, string personId)
    {
        var personsRelated = await Controllers.AccessLevelAndAccessGroup.InformationListOfPersonController.Get(1, 500, 1, privilegeGroupId);
        var personList = personsRelated?.data?.list;

        return personList != null
            ? Array.Find(personList, x => x.id.ToString() == personId)
            : null;
    }

    private static bool ShouldAddPersonToAccess(string[] accessArray, string privilegeGroupId, dynamic personRelated)
    {
        return personRelated == null && accessArray.Contains(privilegeGroupId);
    }

    private static bool ShouldRemovePersonFromAccess(string[]? accessArray, string privilegeGroupId, dynamic personRelated)
    {
        return accessArray is null || personRelated != null && !accessArray.Contains(privilegeGroupId);
    }

    private static Task<Models.AccessLevelAndAccessGroup.EditPerson?> AddPersonToAccess(string personId, string privilegeGroupId)
    {
        return Controllers.AccessLevelAndAccessGroup.EditPersonController.Add(personId, privilegeGroupId);
    }

    private static Task<Models.AccessLevelAndAccessGroup.EditPerson?> RemovePersonFromAccess(string personId, string privilegeGroupId)
    {
        return Controllers.AccessLevelAndAccessGroup.EditPersonController.Delete(personId, privilegeGroupId);
    }

    private static Task<Models.Person.AccessLevel?> ReapplyAccessLevels()
    {
        return Controllers.Person.AccessLevelController.ReApply();
    }
}