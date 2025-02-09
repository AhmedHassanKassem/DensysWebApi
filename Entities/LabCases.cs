public partial class LabCases
{
    public int id_case { get; set; }
    public int id_doctor { get; set; }
    public int id_patient { get; set; }
    public int id_lab { get; set; }
    public int id_proced { get; set; }
    public string remark { set; get; } = null!;
    public double price { set; get; }

    public DateTime date_added { get; set; }
}
public partial class LabCasesDto
{
    public int id_doctor { get; set; }
    public int id_patient { get; set; }
    public int id_lab { get; set; }
    public int id_proced { get; set; }
    public double price { set; get; }
    
    public string remark { set; get; } = null!;
}