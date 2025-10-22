#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections

using Newtonsoft.Json;
using System.Text;

namespace HikCentral.Utils;
public static class Fun
{
    public static HttpRequestMessage GenerateHttpRequestMessage(dynamic body)
    {
        var requestMessage = new HttpRequestMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
        };
        return requestMessage;
    }
    public static HttpClient GenerateHttpClient(string path)
    {
        var stringToSign = $"{Configuration.Setting.StringToSign}{Configuration.Setting.AppKey}\n{path}";
        var signature = Encrypt.HikCentralSignature(Configuration.Setting.AppSecret, stringToSign);
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        HttpClient httpClient = new(handler);
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
        httpClient.DefaultRequestHeaders.Add("x-ca-key", Configuration.Setting.AppKey);
        httpClient.DefaultRequestHeaders.Add("x-ca-signature-headers", "x-ca-key");
        httpClient.DefaultRequestHeaders.Add("x-ca-signature", signature);
        return httpClient;
    }
    public static async Task NotifySuccess(Mail mailService, string customMessage, List<string> recipients, string personName, string dni)
    {
        var subject = $"Correcto - HikCentral - {personName}";
        var body = $"{customMessage}\nDetalles: {personName} DNI {dni}";
        await mailService.SendAsync(recipients, subject, body, false);
    }
    public static async Task HandleException(Exception ex, string customMessage, string dni, Mail mailService, List<string> recipients)
    {
        var subject = $"Error - HikCentral - DNI {dni}";
        var body = $"{customMessage}\nDetalles: {ex.Message}\n\n{ex.StackTrace}";
        await mailService.SendAsync(recipients, subject, body, false);
    }
    public static Mail InitializeMailService(string emailToNotify)
    {
        var mailService = new Mail(
            Configuration.Setting.Sender.Address!,
            Configuration.Setting.Sender.Password!,
            Configuration.Setting.Sender.Ssl,
            Configuration.Setting.Sender.SmtpPort,
            Configuration.Setting.Sender.SmtpServer!
        );
        Configuration.Setting.Recipients.Add(emailToNotify);
        return mailService;
    }
}