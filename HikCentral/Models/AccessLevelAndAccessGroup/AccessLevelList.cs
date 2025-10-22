#pragma warning disable IDE1006 // Estilos de nombres
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.

namespace HikCentral.Models.AccessLevelAndAccessGroup;
public class AccessLevelList
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
        public string privilegeGroupId { get; set; }
        public string privilegeGroupName { get; set; }
        public string description { get; set; }
        public Timeschedule timeSchedule { get; set; }
    }
    public class Timeschedule
    {
        public string indexCode { get; set; }
        public string name { get; set; }
    }
}