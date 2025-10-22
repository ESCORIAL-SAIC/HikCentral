namespace HikCentral.Models.AccessLevelAndAccessGroup;

public partial class InformationListOfPerson
{
    public long code { get; set; }
    public string? msg { get; set; }
    public Data? data { get; set; }
    public partial class Data
    {
        public long total { get; set; }
        public long pageNo { get; set; }
        public long pageSize { get; set; }
        public List[]? list { get; set; }
    }
    public partial class List
    {
        public long id { get; set; }
    }
}