using System;
using System.Collections.Generic;

namespace DensysWebApi.Entities;

public partial class ProceduresDatum
{
    public int id_proced { get; set; }

    public string proced_name { get; set; } = null!;

    public float price { get; set; }
    public float lab_cost { get; set; }
    public float clinic_percent { get; set; }
    public float doctor_percent { get; set; }

    public string? remark { get; set; }

    public DateTime date_added { get; set; }
}
public partial class ProceduresDatumDto
{

    public string proced_name { get; set; } = null!;

    public float price { get; set; }

    public float lab_cost { get; set; }

    public float clinic_percent { get; set; }
    public float doctor_percent { get; set; }

    public string? remark { get; set; }

}
