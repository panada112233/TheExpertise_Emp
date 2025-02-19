using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class Historyleave
{
    public Guid Historyleaveid { get; set; }

    public Guid DocumentId { get; set; }

    public int? LastTotalStickDay { get; set; }

    public int? LastTotalPersonDay { get; set; }

    public int? LastTotalMaternityDaystotal { get; set; }

    public int? LastTotalOrdinationDays { get; set; }

    public int? LastTotalVacationDays { get; set; }

    public int? TotalStickDay { get; set; }

    public int? TotalPersonDay { get; set; }

    public int? TotalMaternityDaystotal { get; set; }

    public int? TotalOrdinationDays { get; set; }

    public int? TotalVacationDays { get; set; }

    public int? SumStickDay { get; set; }

    public int? SumPersonDay { get; set; }

    public int? SumMaternityDaystotal { get; set; }

    public int? SumOrdinationDays { get; set; }

    public int? SumVacationDays { get; set; }
}
