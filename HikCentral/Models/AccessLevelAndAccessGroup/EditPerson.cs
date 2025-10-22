#pragma warning disable IDE1006 // Estilos de nombres
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
#pragma warning disable S2094 // Classes should not be empty

namespace HikCentral.Models.AccessLevelAndAccessGroup;
public class EditPerson
{
    public string code { get; set; }
    public string msg { get; set; }
    public Data data { get; set; }
    public class Data { }
}