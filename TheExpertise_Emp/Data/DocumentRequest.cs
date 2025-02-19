using TheExpertise_Emp.Entities;

namespace TheExpertise_Emp.Data
{

    public class HistoryRequset
    {
        public int? last_total_stickDay { get; set; }
        public int? last_total_personDay { get; set; }
        public int? last_total_maternityDaystotal { get; set; }
        public int? last_total_ordinationDays { get; set; }
        public int? last_total_vacationDays { get; set; }

        public int? total_stickDay { get; set; }
        public int? total_personDay { get; set; }
        public int? total_maternityDaystotal { get; set; }
        public int? total_ordinationDays { get; set; }
        public int? total_vacationDays { get; set; }


        public int? sum_stickDay { get; set; }
        public int? sum_personDay { get; set; }
        public int? sum_maternityDaystotal { get; set; }
        public int? sum_ordinationDays { get; set; }
        public int? sum_vacationDays { get; set; }

    }
    public class DocumentRequest
    {
        public string? userid { get; set; }
        public string? leaveTypeId { get; set; }
        public string? createdate { get; set; }
        public string? fullname { get; set; }
        public string? rolesid { get; set; }
        public string? reason { get; set; }
        public string? startdate { get; set; }
        public string? enddate { get; set; }

        public int? totalleave { get; set; }  // ✅ เปลี่ยนจาก total_start_leave เป็น totalleave
        public string? leavedType { get; set; }  // ✅ เปลี่ยน leaved_date_Type เป็น leavedType
        public string? leaved_startdate { get; set; }
        public string? leaved_enddate { get; set; }
        public int? totalleaved { get; set; }

        public string? friendeContact { get; set; }  // ✅ เปลี่ยน friende_contact เป็น friendeContact
        public string? contact { get; set; }
        public string? workingstart { get; set; }  // ✅ เปลี่ยน workingstartDate เป็น workingstart

        public string? approvedDate { get; set; }
        public string? hrApprovedDate { get; set; }
        public string? sentToHRDate { get; set; }
        public string? documentId { get; set; }

        public string? hrSignature { get; set; }
        public string? managerName { get; set; }
        public string? managerComment { get; set; }
        public HistoryRequset historyRequset { get; set; } = new HistoryRequset();
    }

}
