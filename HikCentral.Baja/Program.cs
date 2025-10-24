using HikCentral.Utils;
using System.CommandLine;

namespace HikCentral.Baja;

class Program
{
    protected Program() { }

    static int Main(string[] args)
    {

        Option<string> dniOption = new("--dni", "-d")
        {
            Description = "DNI de la persona a deshabilitar",
            Required = true,
        };

        Option<string> mailOption = new("--mail", "-m")
        {
            Description = "Correo electrónico para notificaciones",
            Required = true,
        };

        Option<string> beginTimeOption = new("--begin-time", "-t")
        {
            Description = "Hora de inicio para el rango de tiempo (formato yyyy-MM-ddThh:mm:ss)",
            Required = true,
        };

        RootCommand rootCommand = new("Utilidad para deshabilitar persona en HikCentral");

        Command subCommandDisable = new("disable", "Deshabilita a una persona en el sistema de accesos");
        subCommandDisable.Options.Add(dniOption);
        subCommandDisable.Options.Add(mailOption);

        Command subCommandChangeBeginTime = new("change-begintime", "Especifica la hora de inicio para el rango de tiempo");
        subCommandChangeBeginTime.Options.Add(beginTimeOption);
        subCommandChangeBeginTime.Options.Add(dniOption);
        subCommandChangeBeginTime.Options.Add(mailOption);

        rootCommand.Subcommands.Add(subCommandDisable);
        rootCommand.Subcommands.Add(subCommandChangeBeginTime);

        LoadConfiguration();

        subCommandDisable.SetAction(async parseResult =>
        {
            var dni = parseResult.GetValue(dniOption)!;
            var mail = parseResult.GetValue(mailOption)!;

            var mailToNotify = Fun.InitializeMailService(mail);

            await DisablePerson(dni, mailToNotify);
        });

        subCommandChangeBeginTime.SetAction(async parseResult =>
        {
            var dni = parseResult.GetValue(dniOption)!;
            var newBeginTime = parseResult.GetValue(beginTimeOption)!;
            var mail = parseResult.GetValue(mailOption)!;

            await ChangeBeginTime(dni, newBeginTime, mail);
        });

        return rootCommand.Parse(args).Invoke();
    }

    private static async Task DisablePerson(string dni, Mail mailService)
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
    private static async Task ChangeBeginTime(string dni, string stringBeginTime, string mail)
    {
        var recipients = Configuration.Setting.Recipients;
        var mailService = Fun.InitializeMailService(mail);
        var newBeginTime = new DateTimeOffset(DateTime.Parse(stringBeginTime), TimeSpan.FromHours(-3));
        try
        {
            Console.WriteLine("Buscando persona en HikCentral...");
            var person = await Controllers.Person.EditPersonController.FindPersonByDni(dni);
            Console.WriteLine($"Persona encontrada: {person.personName}");
            Console.WriteLine("Modificando hora de inicio...");
            await Controllers.Person.AccessLevelController.ChangePersonBeginTime(person, newBeginTime);
            Console.WriteLine("Hora de inicio modificada correctamente.");
            Console.WriteLine("Enviando mails...");
            await Fun.NotifySuccess(mailService, "Hora de inicio modificada correctamente.", recipients, person.personName, person.cards[0].cardNo);
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
        //Console.ReadKey();
    }

}