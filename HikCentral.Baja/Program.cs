using HikCentral.Utils;

namespace HikCentral.Baja;

class Program
{
    protected Program() { }

    static async Task Main(string[] args)
    {
        try
        {
            LoadConfiguration();
            ValidateArgs(args);

            var dni = args[0];
            var emailToNotify = args[1];
            var mailService = Fun.InitializeMailService(emailToNotify);

            await ProcessPerson(dni, mailService);
        }
        catch (Exception ex)
        {
            HandleGeneralException(ex);
        }
    }

    private static void LoadConfiguration()
    {
        try
        {
            Console.WriteLine("Cargando configuraciones...");
            Configuration.Load();
            Console.WriteLine("Configuraciones cargadas correctamente.");
        }
        catch (SettingsException ex)
        {
            HandleSpecificException(ex, "Error cargando configuraciones.");
        }
        catch (RecipientsException ex)
        {
            HandleSpecificException(ex, "Error cargando destinatarios.");
        }
        catch (SenderException ex)
        {
            HandleSpecificException(ex, "Error cargando remitente.");
        }
    }
    private static void ValidateArgs(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Por favor, proporcione el DNI y/o el correo electrónico deshabilitante.");
            Environment.Exit(1);
        }
    }
    private static async Task ProcessPerson(string dni, Mail mailService)
    {
        var recipients = Configuration.Setting.Recipients;
        try
        {
            Console.WriteLine("Buscando persona en HikCentral...");
            var person = await Controllers.Person.EditPersonController.FindPersonByDni(dni);
            Console.WriteLine($"Persona encontrada: {person.personName}");
            Console.WriteLine("Quitando accesos biométricos y moviendo a grupo Inactivos");
            await Controllers.Person.AccessLevelController.DisablePersonAccess(person);
            Console.WriteLine("Persona deshabilitada completamente del sistema de accesos.");
            Console.WriteLine("Enviando mails...");
            await Fun.NotifySuccess(mailService, "Persona deshabilitada correctamente del sistema de accesos.", recipients, person.personName, person.cards[0].cardNo);
            Console.WriteLine("Mails enviados.");
        }
        catch (Exception ex) when (
            ex is PersonInformationNotRetrievedException ||
            ex is PersonNotFoundException ||
            ex is AccessLevelAndAccessGroupNotRetrievedException ||
            ex is AccessLevelReApplyException ||
            ex is InvalidOperationException
        )
        {
            await HandleSpecificException(ex, dni, mailService, recipients);
        }
    }
    private static void HandleGeneralException(Exception ex)
    {
        Console.WriteLine($"Error desconocido.\n{ex.Message}\nPresione una tecla para finalizar...");
        Console.ReadKey();
    }
    private static void HandleSpecificException(Exception ex, string message)
    {
        Console.WriteLine($"{message}\n{ex.Message}\nPresione una tecla para finalizar...");
        Console.ReadKey();
        Environment.Exit(1);
    }
    private static async Task HandleSpecificException(Exception ex, string dni, Mail mailService, List<string> recipients)
    {
        Console.WriteLine(ex.Message);
        string subject = ex switch
        {
            PersonInformationNotRetrievedException => "No se pudo obtener la información de la persona",
            PersonNotFoundException => "Persona no encontrada",
            AccessLevelAndAccessGroupNotRetrievedException => "No se pudieron obtener los grupos de acceso",
            AccessLevelReApplyException => "Error al aplicar cambios en dispositivos de acceso",
            InvalidOperationException => "Error al mover a la persona al grupo 'Inactivos'",
            _ => "Error inesperado"
        };
        await Fun.HandleException(ex, subject, dni, mailService, recipients);
        Console.ReadKey();
    }
}