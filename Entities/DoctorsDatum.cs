using System;
using System.Collections.Generic;

namespace DensysWebApi.Entities;

public partial class DoctorsDatum
{
    public int id_doctor { get; set; }

    public string doctor_name { get; set; } = null!;

    public string phone_number { get; set; } = null!;

    public string address { get; set; } = null!;
    public string specialization { get; set; } = null!;

    public string remark { get; set; } = null!;

    public DateTime date_added { get; set; }
}
public partial class DoctorsDatumDto
{

    public string doctor_name { get; set; } = null!;

    public string phone_number { get; set; } = null!;

    public string address { get; set; } = null!;
    public string specialization { get; set; } = null!;

    public string remark { get; set; } = null!;

}
