using System;
using System.Collections.Generic;

namespace DensysWebApi.Entities;

public partial class Appointments
{
    public int id_appoint { get; set; }

    public int id_patient { get; set; }
    public int id_doctor { get; set; }
    public int id_proced { get; set; }
      public double amount_paid { get; set; }
      public double remaining_amount { get; set; }
    public DateTime appoint_date { get; set; }

    public TimeSpan from_hour { get; set; }
    public TimeSpan to_hour { get; set; }

    public bool? Complete { get; set; }
    public string remark { get; set; } = null!;


    public DateTime DateAdded { get; set; }
}
public partial class AppointmentsDto
{

    public int id_patient { get; set; }

     public int id_doctor { get; set; }
    public int id_proced { get; set; }
    public DateTime appoint_date { get; set; }

    public TimeSpan from_hour { get; set; }
    public TimeSpan to_hour { get; set; }
  public double amount_paid { get; set; }
      public double remaining_amount { get; set; }
    public bool? Complete { get; set; }
    public string remark { get; set; } = null!;


}
