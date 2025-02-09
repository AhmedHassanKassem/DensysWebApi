using System;
using System.Collections.Generic;

namespace DensysWebApi.Entities;

public partial class LabDatum
{
    public int id_lab { get; set; }

    public int id_proced { get; set; }

    public string lab_name { get; set; } = null!;

    public double lab_fee { get; set; }
    public string remark { get; set; }=null!;

    public DateTime date_added { get; set; }
}
public partial class LabDatumDto
{

    public int id_proced { get; set; }

    public string lab_name { get; set; } = null!;

    public double lab_fee { get; set; }
    public string remark { get; set; }=null!;

}
