using System;
using System.Collections.Generic;

namespace DensysWebApi.Entities;

public partial class PatientDatum
{
    public int id_patient { get; set; }

    public int id_doctor { get; set; }
    public int patient_no { get; set; }
    public int patient_age { get; set; }
    public string name { get; set; } = null!;
    public double price { get; set; }
  

    public string patient_gender { get; set; } = null!;
    public string chronic_disease { get; set; } = null!;
    public string phone_1 { get; set; } = null!;
    public string phone_2 { get; set; } = null!;
    public string address { get; set; } = null!;

    public string remark { get; set; } = null!;

    public DateTime DateAdded { get; set; }
}
public partial class PatientDatumDto
{
     public int id_doctor { get; set; }
    public int patient_no { get; set; }
    public int patient_age { get; set; }
    public string name { get; set; } = null!;
    public double price { get; set; }
    public string patient_gender { get; set; } = null!;
    public string chronic_disease { get; set; } = null!;
    public string phone_1 { get; set; } = null!;
    public string phone_2 { get; set; } = null!;
    public string address { get; set; } = null!;

    public string remark { get; set; } = null!;

}
