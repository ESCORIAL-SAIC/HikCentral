using LiteDB;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace HikCentral.Utils;

public class SettingData
{
    public string? ServerAddress { get; set; }
    public string? AppKey { get; set; }
    public string? AppSecret { get; set; }
    public string? StringToSign { get; set; }
}
public class Recipient
{
    public string? Address { get; set; }
}
public class Sender
{
    public string? Address { get; set; }
    public string? Password { get; set; }
    public bool Ssl { get; set; }
    public int SmtpPort { get; set; }
    public string? SmtpServer { get; set; }
}
public class Endpoint
{
    public ApiPath? GetAccessLevelList { get; set; }
    public ApiPath? DeletePersonFromAccessLevel { get; set; }
    public ApiPath? AddPersonToAccessLevel { get; set; }
    public ApiPath? ReApplyAccessSettings { get; set; }
    public ApiPath? PersonUpdate { get; set; }
    public ApiPath? PersonGet { get; set; }
    public ApiPath? GetPersonsRelatedToAccessLevel { get; set; }
}
public class ApiPath
{
    public string? Name { get; set; }
    public string? Path { get; set; }
}
public static class Configuration
{
    public static class Setting
    {
        public static string? ServerAddress { get; set; }
        public static string? AppKey { get; set; }
        public static string? AppSecret { get; set; }
        public static string? StringToSign { get; set; }
        public static List<string> Recipients { get; set; } = [];
        public static Sender Sender { get; set; } = new();
        public static Endpoint Endpoint { get; set; } = new();
    }
    public static class ConnectionString
    {
        public static string LiteDb
        {
            get
            {
                var builder = new ConfigurationBuilder()
                 .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"));
                var config = builder.Build();
                return config["connectionString:liteDb"]!;
            }
        }
        public static string Escorial
        {
            get
            {
                var builder = new ConfigurationBuilder()
                 .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"));
                var config = builder.Build();
                return config["connectionString:escorial"]!;
            }
        }
    }

    public static void Load()
    {
        var liteDbConnectionString = ConnectionString.LiteDb;
        using var db = new LiteDatabase(liteDbConnectionString);
        var settingsDb = db.GetCollection<SettingData>("config");
        var configData =
            settingsDb.Query().FirstOrDefault() ??
            throw new SettingsException("No se cargaron correctamente las configuraciones");
        Setting.ServerAddress = configData.ServerAddress;
        Setting.AppKey = configData.AppKey;
        Setting.AppSecret = configData.AppSecret;
        Setting.StringToSign = configData.StringToSign;
        var recipientsDb = db.GetCollection<Recipient>("recipients");
        var recipientsData = recipientsDb
            .Query()
            .Select(r => r) ??
            throw new RecipientsException("No se cargaron correctamente los destinatarios");
        var recipients = new List<string>();
        recipientsData
            .ToList()
            .ForEach(r => recipients.Add(r.Address ?? throw new RecipientsException("No se cargaron correctamente los destinatarios")));
        Setting.Recipients = recipients;
        var senderDb = db.GetCollection<Sender>("sender");
        var senderData = senderDb
            .Query()
            .FirstOrDefault() ??
            throw new SenderException("Error cargando el remitente");
        Setting.Sender.Address = senderData.Address;
        Setting.Sender.Password = senderData.Password;
        Setting.Sender.Ssl = senderData.Ssl;
        Setting.Sender.SmtpServer = senderData.SmtpServer;
        Setting.Sender.SmtpPort = senderData.SmtpPort;
        var apiPathDb = db.GetCollection<ApiPath>("apiPath");
        var apiPathData = apiPathDb
            .Query()
            .Select(r => r)
            .ToList() ??
            throw new ApiPathException("No se puedieron cargar correctamente las rutas de la API.");
        Setting.Endpoint.GetAccessLevelList = apiPathData.Find(e => e.Name == nameof(Endpoint.GetAccessLevelList));
        Setting.Endpoint.DeletePersonFromAccessLevel = apiPathData.Find(e => e.Name == nameof(Endpoint.DeletePersonFromAccessLevel));
        Setting.Endpoint.AddPersonToAccessLevel = apiPathData.Find(e => e.Name == nameof(Endpoint.AddPersonToAccessLevel));
        Setting.Endpoint.ReApplyAccessSettings = apiPathData.Find(e => e.Name == nameof(Endpoint.ReApplyAccessSettings));
        Setting.Endpoint.PersonUpdate = apiPathData.Find(e => e.Name == nameof(Endpoint.PersonUpdate));
        Setting.Endpoint.PersonGet = apiPathData.Find(e => e.Name == nameof(Endpoint.PersonGet));
        Setting.Endpoint.GetPersonsRelatedToAccessLevel = apiPathData.Find(e => e.Name == nameof(Endpoint.GetPersonsRelatedToAccessLevel));
    }
}