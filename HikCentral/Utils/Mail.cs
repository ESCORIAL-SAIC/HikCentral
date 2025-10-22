using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HikCentral.Utils;

/// <summary>
/// Clase para envío de correos electrónicos.
/// </summary>
/// <remarks>
/// Inicializa una nueva instancia de la clase <see cref="Mail"/>.
/// </remarks>
/// <param name="sender">Dirección desde la cual se envía el correo.</param>
/// <param name="password">Clave de mail desde el cual se envía el correo.</param>
/// <param name="ssl">Indica si utiliza el protocolo seguro SSL.</param>
/// <param name="port">Puerto para conexión con servidor SMTP.</param>
/// <param name="smtpServer">Dirección del servidor SMTP.</param>
public partial class Mail(string sender, string password, bool ssl, int port, string smtpServer)
{
    /// <summary>
    /// Dirección desde la cual se envía el correo.
    /// </summary>
    public string Sender { get; private set; } = sender;

    /// <summary>
    /// Clave de mail desde el cual se envía el correo.
    /// </summary>
    public string Password { get; private set; } = password;

    /// <summary>
    /// Indica si utiliza el protocolo seguro SSL.
    /// </summary>
    public bool Ssl { get; private set; } = ssl;

    /// <summary>
    /// Puerto para conexión con servidor SMTP.
    /// </summary>
    public int Port { get; private set; } = port;

    /// <summary>
    /// Dirección del servidor SMTP.
    /// </summary>
    public string SmtpServer { get; private set; } = smtpServer;

    /// <summary>
    /// Envía un correo electrónico de manera asíncrona.
    /// </summary>
    /// <param name="recipients">Lista de destinatarios.</param>
    /// <param name="subject">Asunto del correo.</param>
    /// <param name="body">Cuerpo del correo.</param>
    /// <param name="isHtml">Indica si el cuerpo es HTML.</param>
    /// <param name="cc">Lista de direcciones en copia.</param>
    /// <param name="bcc">Lista de direcciones en copia oculta.</param>
    /// <returns>Retorna true si el correo fue enviado con éxito.</returns>
    public async Task SendAsync(List<string> recipients, string subject, string body, bool isHtml, List<string>? cc = null, List<string>? bcc = null)
    {
        var mail = new MailMessage
        {
            From = new MailAddress(Sender),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        recipients.ForEach(mail.To.Add);
        cc?.ForEach(mail.CC.Add);
        bcc?.ForEach(mail.Bcc.Add);

        using var smtp = new SmtpClient
        {
            EnableSsl = Ssl,
            Port = Port,
            Host = SmtpServer,
            Credentials = new NetworkCredential(Sender, Password)
        };

        await smtp.SendMailAsync(mail);
    }

    [GeneratedRegex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
    private static partial Regex MailRegex();

    /// <summary>
    /// Valida la dirección de correo proporcionada.
    /// </summary>
    /// <param name="address">Dirección de correo electrónico a verificar.</param>
    /// <returns>True si la dirección es válida, de lo contrario false.</returns>
    public static bool ValidateAddress(string address)
    {
        if (address.EndsWith('.'))
            return false;
        var regex = MailRegex();
        if (!regex.IsMatch(address))
            return false;
        try
        {
            return new MailAddress(address) is not null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Valida la dirección de correo proporcionada con una expresión regular personalizada.
    /// </summary>
    /// <param name="address">Dirección de correo electrónico a verificar.</param>
    /// <param name="regex">Expresión regular a utilizar para la validación.</param>
    /// <returns>True si la dirección es válida, de lo contrario false.</returns>
    public static bool ValidateAddress(string address, string regex)
    {
        if (address.EndsWith('.'))
            return false;
        try
        {
            return Regex.IsMatch(address, regex);
        }
        catch
        {
            return false;
        }
    }
}
