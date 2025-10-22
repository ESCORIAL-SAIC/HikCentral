#pragma warning disable IDE1006 // Estilos de nombres
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.

namespace HikCentral.Models.Person;
public class AccessLevel
{
    public string code { get; set; }
    public string msg { get; set; }
    public string data { get; set; }
}