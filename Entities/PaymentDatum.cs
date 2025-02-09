using System;
using System.Collections.Generic;

namespace DensysWebApi.Entities;

public partial class PaymentDatum
{
    public int IdPayment { get; set; }

    public int AppointId { get; set; }

    public double? Total { get; set; }

    public double? Paid { get; set; }

    public double? RemainingAmount { get; set; }

    public DateTime DateAdded { get; set; }
}
