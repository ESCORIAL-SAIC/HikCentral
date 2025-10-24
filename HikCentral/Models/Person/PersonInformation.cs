#pragma warning disable IDE1006 // Estilos de nombres
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.

namespace HikCentral.Models.Person;
public class PersonInformation
{
    public string code { get; set; }
    public string msg { get; set; }
    public Data data { get; set; }

    public class Data
    {
        public int total { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public List[] list { get; set; }
    }
    public class List
    {
        public string personId { get; set; }
        public string personCode { get; set; }
        public string orgIndexCode { get; set; }
        public string personName { get; set; }
        public string personFamilyName { get; set; }
        public string personGivenName { get; set; }
        public int gender { get; set; }
        public string phoneNo { get; set; }
        public Personphoto personPhoto { get; set; }
        public string email { get; set; }
        public string remark { get; set; }
        public DateTimeOffset beginTime { get; set; }
        public DateTimeOffset endTime { get; set; }
        public Card[] cards { get; set; }
        public Fingerprint[] fingerPrint { get; set; }
    }
    public class Personphoto
    {
        public string picUri { get; set; }
    }
    public class Card
    {
        public string cardNo { get; set; }
    }
    public class Fingerprint
    {
        public string fingerPrintIndexCode { get; set; }
        public string fingerPrintName { get; set; }
        public string fingerPrintData { get; set; }
        public string relatedCardNo { get; set; }
    }
}