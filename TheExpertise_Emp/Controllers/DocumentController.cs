using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.DTO;
using TheExpertise_Emp.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Globalization;
using TheExpertise_Emp.Models;



namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly EmployeeDocumentDbsContextNew _dbcontext;
        private readonly IConfiguration _configuration; // ✅ เพิ่ม IConfiguration

        public DocumentController(EmployeeDocumentDbsContextNew dbsContextNew, IConfiguration configuration)
        {
            _dbcontext = dbsContextNew;
            _configuration = configuration; // ✅ กำหนดค่าให้ _configuration
        }

        // ✅ ดึงรายการเอกสารของพนักงานตาม UserID
        [HttpGet("GetDocumentsByUser/{userId}")]
        public async Task<IActionResult> GetDocumentsByUser(int userId)
        {
            Console.WriteLine($"📌 Fetching documents for UserID: {userId}");

            var documents = await _dbcontext.Documents
                .Where(d => d.UserId == userId && d.Status != "Commited") // ✅ เพิ่มเงื่อนไขไม่ให้ดึง Commited
                .ToListAsync();

            if (documents == null || documents.Count == 0)
            {
                Console.WriteLine("❌ ไม่พบเอกสารของพนักงาน");
                return NotFound("ไม่พบเอกสารของพนักงานคนนี้");
            }

            Console.WriteLine($"✅ พบเอกสารทั้งหมด {documents.Count} รายการ");
            return Ok(documents);
        }


        [HttpGet("GetLeaveTypes")]
        public async Task<IActionResult> GetLeaveTypes()
        {
            try
            {
                var data = await _dbcontext.LeaveTypes.ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "เกิดข้อผิดพลาดในการดึงข้อมูล", error = ex.Message });
            }
        }
        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var data = await _dbcontext.Roles.ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "เกิดข้อผิดพลาดในการดึงข้อมูล", error = ex.Message });
            }
        }


        // ✅ สร้างเอกสารใหม่ (ใบลา)
        [HttpPost("CreateDocument")]
        public async Task<IActionResult> CreateDocument(TheExpertise_Emp.Data.DocumentRequest documentreq)  // ✅ ระบุ namespace ให้ชัดเจน

        {
            try
            {
                if (documentreq == null)
                {
                    return BadRequest("ข้อมูลเอกสารไม่ถูกต้อง");
                }


                var docid = Guid.NewGuid();

                string dateStr = "2022/05/19";
                string createdatenew = documentreq.createdate.Replace("-", "/");
                DateTime? dateTime;
                dateTime = DateTime.ParseExact(dateStr, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                dateTime = dateTime.Value.Date.Add(DateTime.Now.TimeOfDay);

                var documentinsert = new Document()
                {
                    UserId = !string.IsNullOrEmpty(documentreq.userid) ? int.Parse(documentreq.userid) : 00,
                    DocumentId = docid,
                    Status = "draft",  // ✅ เพิ่มค่าเริ่มต้นให้สถานะเป็น "draft" แทน "pending_manager"

                    LeaveTypeId = !string.IsNullOrEmpty(documentreq.leaveTypeId) ? Guid.Parse(documentreq.leaveTypeId) : Guid.NewGuid(),
                    Rolesid = !string.IsNullOrEmpty(documentreq.rolesid) ? Guid.Parse(documentreq.rolesid) : Guid.NewGuid(),
                    LeavedType = !string.IsNullOrEmpty(documentreq.leavedType) ? Guid.Parse(documentreq.leavedType) : Guid.NewGuid(),
                    Fullname = documentreq.fullname,
                    Reason = documentreq.reason,
                    ApprovedDate = !string.IsNullOrEmpty(documentreq.approvedDate) ? helperFucntion.ConvertStrToDate(documentreq.approvedDate) : DateTime.Now,
                    Contact = documentreq.contact,

                    Createdate = !string.IsNullOrEmpty(documentreq.createdate) ? helperFucntion.ConvertStrToDate(documentreq.createdate) : DateTime.Now,
                    Startdate = !string.IsNullOrEmpty(documentreq.startdate) ? helperFucntion.ConvertStrToDate(documentreq.startdate) : DateTime.Now,

                    Enddate = !string.IsNullOrEmpty(documentreq.enddate) ? helperFucntion.ConvertStrToDate(documentreq.enddate) : DateTime.Now,

                    FriendeContact = documentreq.friendeContact,
                    Workingstart = !string.IsNullOrEmpty(documentreq.workingstart) ? helperFucntion.ConvertStrToDate(documentreq.workingstart) : DateTime.Now,
                    HrApprovedDate = !string.IsNullOrEmpty(documentreq.hrApprovedDate) ? helperFucntion.ConvertStrToDate(documentreq.hrApprovedDate) : DateTime.Now,
                    Totalleave = documentreq.totalleave,
                    HrSignature = documentreq.hrSignature,
                    LeavedEnddate = !string.IsNullOrEmpty(documentreq.leaved_enddate) ? helperFucntion.ConvertStrToDate(documentreq.leaved_enddate) : DateTime.Now,
                    LeavedStartdate = !string.IsNullOrEmpty(documentreq.leaved_startdate) ? helperFucntion.ConvertStrToDate(documentreq.leaved_startdate) : DateTime.Now,
                    ManagerComment = documentreq.managerComment,
                    ManagerName = documentreq.managerName,
                    SentToHrdate = !string.IsNullOrEmpty(documentreq.sentToHRDate) ? helperFucntion.ConvertStrToDate(documentreq.sentToHRDate) : DateTime.Now,
                    Totalleaved = documentreq.totalleaved
                };

                await _dbcontext.Documents.AddAsync(documentinsert);
                var resultSave = await _dbcontext.SaveChangesAsync();
                if (resultSave > 0)
                {
                    var history = new Historyleave()
                    {
                        DocumentId = docid,
                        LastTotalMaternityDaystotal = documentreq.historyRequset.last_total_maternityDaystotal ?? 0,
                        LastTotalOrdinationDays = documentreq.historyRequset.last_total_ordinationDays ?? 0,
                        LastTotalPersonDay = documentreq.historyRequset.last_total_personDay ?? 0,
                        LastTotalStickDay = documentreq.historyRequset.last_total_stickDay ?? 0,
                        LastTotalVacationDays = documentreq.historyRequset.last_total_vacationDays ?? 0,
                        SumMaternityDaystotal = documentreq.historyRequset.sum_maternityDaystotal ?? 0,
                        SumOrdinationDays = documentreq.historyRequset.sum_ordinationDays ?? 0,
                        SumPersonDay = documentreq.historyRequset.sum_personDay ?? 0,
                        SumStickDay = documentreq.historyRequset.sum_stickDay ?? 0,
                        SumVacationDays = documentreq.historyRequset.sum_vacationDays ?? 0,
                        TotalMaternityDaystotal = documentreq.historyRequset.total_maternityDaystotal ?? 0,
                        TotalOrdinationDays = documentreq.historyRequset.total_ordinationDays ?? 0,
                        TotalPersonDay = documentreq.historyRequset.total_personDay ?? 0,
                        TotalStickDay = documentreq.historyRequset.total_stickDay ?? 0,
                        TotalVacationDays = documentreq.historyRequset.total_vacationDays ?? 0,
                    };

                    await _dbcontext.Historyleaves.AddAsync(history);
                    var historySave = await _dbcontext.SaveChangesAsync();
                    return historySave > 0 ? Ok(new { message = "success", documentid = docid }) : BadRequest("บันทึกข้อมูลล้มเหลว");

                }
                else
                {
                    return BadRequest("Saving history Fail");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาด: " + ex.Message);
            }
        }
        [HttpGet("GetCommitedDocumentsByUser/{userId}")]
        public async Task<IActionResult> GetCommitedDocumentsByUser(int userId)
        {
            Console.WriteLine($"📌 Fetching Commited documents for UserID: {userId}");

            var documents = await _dbcontext.Documents
                .Where(d => d.UserId == userId && d.Status == "Commited") // ✅ ดึงเฉพาะที่เป็น "Commited"
                .ToListAsync();

            if (documents == null || documents.Count == 0)
            {
                Console.WriteLine("❌ ไม่พบเอกสารที่ได้รับการอนุมัติแล้ว");
                return NotFound("ไม่พบเอกสารที่ได้รับการอนุมัติแล้ว");
            }

            Console.WriteLine($"✅ พบเอกสารทั้งหมด {documents.Count} รายการ");
            return Ok(documents);
        }
        [HttpGet("GetAllCommitedDocuments")]
        public async Task<IActionResult> GetAllCommitedDocuments()
        {
            Console.WriteLine("📌 Fetching all Commited documents for all users");

            // ✅ ดึงเอกสารที่ได้รับการอนุมัติแล้ว และต้องมี UserId ที่ถูกต้อง (มากกว่า 0)
            var documents = await _dbcontext.Documents
                .Where(d => d.Status == "Commited" && d.UserId != null && d.UserId > 0)
                .ToListAsync();

            if (documents == null || documents.Count == 0)
            {
                Console.WriteLine("❌ ไม่พบเอกสารที่ได้รับการอนุมัติแล้ว");
                return NotFound("ไม่พบเอกสารที่ได้รับการอนุมัติแล้ว");
            }

            Console.WriteLine($"✅ พบเอกสารทั้งหมด {documents.Count} รายการ");

            // ✅ ตรวจสอบและกรองเอกสารที่มีพนักงานที่ไม่รู้จัก
            var filteredDocuments = documents.Where(d =>
            {
                var user = _dbcontext.Users.FirstOrDefault(u => u.UserId == d.UserId);
                if (user == null)
                {
                    Console.WriteLine($"⚠️ ข้ามเอกสารของพนักงานที่ไม่รู้จัก: {d.DocumentId}");
                    return false; // ไม่รวมเอกสารของพนักงานที่ไม่มีในระบบ
                }
                return true;
            }).ToList();

            Console.WriteLine($"📌 หลังกรองแล้ว เหลือ {filteredDocuments.Count} เอกสาร");

            return Ok(filteredDocuments);
        }


        [HttpPut("UpdateDocument/{documentId}")]
        public async Task<IActionResult> UpdateDocument(Guid documentId, TheExpertise_Emp.Data.DocumentRequest documentreq)
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(documentId);
                if (document == null)
                {
                    return NotFound("❌ ไม่พบเอกสารที่ต้องการอัปเดต");
                }

                // ✅ อัปเดตข้อมูลหลักของเอกสาร
                document.LeaveTypeId = !string.IsNullOrEmpty(documentreq.leaveTypeId) ? Guid.Parse(documentreq.leaveTypeId) : document.LeaveTypeId;
                document.Rolesid = !string.IsNullOrEmpty(documentreq.rolesid) ? Guid.Parse(documentreq.rolesid) : document.Rolesid;
                document.LeavedType = !string.IsNullOrEmpty(documentreq.leavedType) ? Guid.Parse(documentreq.leavedType) : document.LeavedType;
                document.Fullname = documentreq.fullname ?? document.Fullname;
                document.Reason = documentreq.reason ?? document.Reason;
                document.Startdate = !string.IsNullOrEmpty(documentreq.startdate) ? helperFucntion.ConvertStrToDate(documentreq.startdate) : document.Startdate;
                document.Enddate = !string.IsNullOrEmpty(documentreq.enddate) ? helperFucntion.ConvertStrToDate(documentreq.enddate) : document.Enddate;
                document.Totalleave = documentreq.totalleave;
                document.Contact = documentreq.contact ?? document.Contact;
                document.Createdate = !string.IsNullOrEmpty(documentreq.createdate) ? helperFucntion.ConvertStrToDate(documentreq.createdate) : document.Createdate;
                document.LeavedStartdate = !string.IsNullOrEmpty(documentreq.leaved_startdate) ? helperFucntion.ConvertStrToDate(documentreq.leaved_startdate) : document.LeavedStartdate;
                document.LeavedEnddate = !string.IsNullOrEmpty(documentreq.leaved_enddate) ? helperFucntion.ConvertStrToDate(documentreq.leaved_enddate) : document.LeavedEnddate;
                document.Totalleaved = documentreq.totalleaved;
                document.FriendeContact = documentreq.friendeContact ?? document.FriendeContact;
                document.Workingstart = !string.IsNullOrEmpty(documentreq.workingstart) ? helperFucntion.ConvertStrToDate(documentreq.workingstart) : document.Workingstart;
                document.ApprovedDate = !string.IsNullOrEmpty(documentreq.approvedDate) ? helperFucntion.ConvertStrToDate(documentreq.approvedDate) : document.ApprovedDate;
                document.HrApprovedDate = !string.IsNullOrEmpty(documentreq.hrApprovedDate) ? helperFucntion.ConvertStrToDate(documentreq.hrApprovedDate) : document.HrApprovedDate;
                document.SentToHrdate = !string.IsNullOrEmpty(documentreq.sentToHRDate) ? helperFucntion.ConvertStrToDate(documentreq.sentToHRDate) : document.SentToHrdate;
                document.HrSignature = documentreq.hrSignature ?? document.HrSignature;
                document.ManagerName = documentreq.managerName ?? document.ManagerName;
                document.ManagerComment = documentreq.managerComment ?? document.ManagerComment;

                // ✅ อัปเดตประวัติการลา (Historyleave)
                var history = await _dbcontext.Historyleaves.FirstOrDefaultAsync(h => h.DocumentId == documentId);
                if (history != null && documentreq.historyRequset != null)
                {
                    // 🔹 ใช้ ?? 0 ป้องกันปัญหาค่าว่าง ("" หรือ null)
                    history.LastTotalVacationDays = documentreq.historyRequset?.last_total_vacationDays ?? 0;
                    history.LastTotalStickDay = documentreq.historyRequset?.last_total_stickDay ?? 0;
                    history.LastTotalPersonDay = documentreq.historyRequset?.last_total_personDay ?? 0;
                    history.LastTotalMaternityDaystotal = documentreq.historyRequset?.last_total_maternityDaystotal ?? 0;
                    history.LastTotalOrdinationDays = documentreq.historyRequset?.last_total_ordinationDays ?? 0;
                    history.TotalStickDay = documentreq.historyRequset?.total_stickDay ?? 0;
                    history.TotalPersonDay = documentreq.historyRequset?.total_personDay ?? 0;
                    history.TotalMaternityDaystotal = documentreq.historyRequset?.total_maternityDaystotal ?? 0;
                    history.TotalOrdinationDays = documentreq.historyRequset?.total_ordinationDays ?? 0;
                    history.TotalVacationDays = documentreq.historyRequset?.total_vacationDays ?? 0;
                    history.SumStickDay = documentreq.historyRequset?.sum_stickDay ?? 0;
                    history.SumPersonDay = documentreq.historyRequset?.sum_personDay ?? 0;
                    history.SumMaternityDaystotal = documentreq.historyRequset?.sum_maternityDaystotal ?? 0;
                    history.SumOrdinationDays = documentreq.historyRequset?.sum_ordinationDays ?? 0;
                    history.SumVacationDays = documentreq.historyRequset?.sum_vacationDays ?? 0;
                }

                await _dbcontext.SaveChangesAsync(); // ✅ บันทึกการเปลี่ยนแปลงลงฐานข้อมูล

                return Ok(new { message = "✅ อัปเดตข้อมูลสำเร็จ!", documentId = documentId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ เกิดข้อผิดพลาด: " + ex.Message);
            }
        }



        [HttpPost("SendDocumentToEmployee")]
        public async Task<IActionResult> SendDocumentToEmployee(ApprovedRequest approvedRequest)
        {
            try
            {
                if (approvedRequest.userId == null)
                {
                    return BadRequest("User id Is NUll");
                }

                if (approvedRequest.documentId == null)
                {
                    return BadRequest("documentId id Is NUll");
                }

                var doc = await _dbcontext.Documents.FirstOrDefaultAsync(x => x.DocumentId == Guid.Parse(approvedRequest.documentId));
                
                if(doc == null)
                    return BadRequest("documentId  Is NUll");

                doc.Status = "Commited";
                
               var issave =  await _dbcontext.SaveChangesAsync();
                if (issave > 0 )
                {
                    return Ok(new { message = "✅ ส่งเอกสารให้พนักงานสำเร็จ!" });
                }
                else
                {
                    return Ok(new { message = " ส่งเอกสารไม่สำเร็จ!" });
                }
           
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex}");
                return StatusCode(500, "❌ เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        [HttpGet("Document")]
        public async Task<IActionResult> GetAllDocument(int userID)
        {
            if (userID <= 0) // ตรวจสอบว่า userID ถูกส่งมาหรือไม่
            {
                return BadRequest("UserID is required and must be a valid integer.");
            }

            var documents = await _dbcontext.Documents.Where(x => x.UserId == userID).ToListAsync();

            return Ok(documents); // ส่งข้อมูลทั้งหมดในรูปแบบ JSON
        }


        [HttpPatch("SendToManager/{documentId}")]
        public async Task<IActionResult> SendToManager(string documentId)
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(Guid.Parse(documentId));
                if (document == null)
                {
                    return NotFound("❌ ไม่พบเอกสารที่ต้องการอัปเดต");
                }
                document.Status = "pending_manager"; // ✅ ให้ตรงกับที่ Manager ใช้ดึงข้อมูล

                await _dbcontext.SaveChangesAsync();
                return Ok(new { message = "ส่งฟอร์มไปยัง GM สำเร็จ!", documentId = document.DocumentId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        [HttpGet("GetPendingFormsForManager")]
        public async Task<IActionResult> GetPendingFormsForManager()
        {
            var documents = await _dbcontext.Documents
                .Where(d => d.Status == "pending_manager")
                .ToListAsync();

            if (documents.Count == 0)
            {
                Console.WriteLine("❌ ไม่พบเอกสารที่ต้องเซ็นโดย GM");
                return NotFound("ไม่พบเอกสารที่ต้องเซ็นโดย GM");
            }

            Console.WriteLine($"✅ พบเอกสาร {documents.Count} รายการที่ต้องเซ็นโดย GM");
            return Ok(documents);
        }


        // ✅ GM อนุมัติเอกสาร
        [HttpPost("ApproveByManager")]
        public async Task<IActionResult> ApproveByManager([FromBody] ApproveRequest request)
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(request.DocumentID);
                if (document == null)
                {
                    return NotFound("❌ ไม่พบเอกสารที่ต้องการอนุมัติ");
                }

                document.ApprovedDate = DateTime.Now;
                document.ManagerName = request.ManagerName;
                document.ManagerComment = request.ManagerComment;
                document.Status = "manager_approved"; // ✅ เปลี่ยนเป็น "manager_approved"

                await _dbcontext.SaveChangesAsync();
                return Ok(new { message = "✅ อนุมัติฟอร์มโดย GM สำเร็จ!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ เกิดข้อผิดพลาด: " + ex.Message);
            }
        }


        [HttpPatch("SendToHR/{documentId}")]
        public async Task<IActionResult> SendToHR(string documentId)
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(Guid.Parse(documentId));
                if (document == null)
                {
                    return NotFound("❌ ไม่พบเอกสารที่ต้องการส่งไป HR");
                }

                document.Status = "pending_hr"; // ✅ เปลี่ยนสถานะให้ HR เห็นเอกสาร
                document.SentToHrdate = DateTime.Now; // บันทึกเวลาส่งให้ HR

                await _dbcontext.SaveChangesAsync();
                return Ok(new { message = "✅ ส่งฟอร์มไป HR สำเร็จ!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ เกิดข้อผิดพลาด: " + ex.Message);
            }
        }


        [HttpGet("GetPendingFormsForHR")]
        public async Task<IActionResult> GetPendingFormsForHR()
        {
            try
            {
                var pendingForms = await _dbcontext.Documents
                    .Where(d => d.Status == "pending_hr") // ✅ ดึงเฉพาะเอกสารที่รออนุมัติจาก HR
                    .ToListAsync();

                return Ok(pendingForms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาด: " + ex.Message);
            }
        }


        [HttpGet("GetDocbyStatus/{docstatus}")]
        public async Task<IActionResult> GetDocbyStatus(string docstatus)
        {
            var documents = await _dbcontext.Documents
                .Where(d => d.Status == docstatus)
                .ToListAsync();

            if (documents.Count == 0)
            {
                return NotFound("❌ ไม่พบเอกสารที่ต้องเซ็นโดย HR");
            }

            return Ok(documents);
        }
        // ✅ HR อนุมัติเอกสาร
        [HttpPost("ApproveByHR")]
        public async Task<IActionResult> ApproveByHR(ApproveRequest request)
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(request.DocumentID);
                if (document == null)
                {
                    return NotFound("❌ ไม่พบเอกสารที่ต้องการอนุมัติ");
                }

                Console.WriteLine($"📌 HR Signature ที่ได้รับ: {request.HRSignature}");

                document.HrApprovedDate = DateTime.Now;
                document.HrSignature = request.HRSignature;
                document.Status = "hr_approved"; // ✅ เปลี่ยนเป็น "hr_approved"

                await _dbcontext.SaveChangesAsync();

                return Ok(new { message = "✅ อนุมัติฟอร์มโดย HR สำเร็จ!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "❌ เกิดข้อผิดพลาด: " + ex.Message);
            }
        }
        [HttpGet("GetApprovedFormsForManager")]
        public async Task<IActionResult> GetApprovedFormsForManager()
        {
            var documents = await _dbcontext.Documents
                .Where(d => d.Status == "manager_approved") // ✅ โหลดเฉพาะเอกสารที่ GM อนุมัติแล้ว
                .ToListAsync();

            if (documents.Count == 0)
            {
                Console.WriteLine("❌ ไม่พบเอกสารที่ GM อนุมัติแล้ว");
                return NotFound("ไม่พบเอกสารที่ GM อนุมัติแล้ว");
            }

            Console.WriteLine($"✅ พบเอกสาร {documents.Count} รายการที่ GM อนุมัติแล้ว");
            return Ok(documents);
        }


        [HttpPut("EditHRSignature")]
        public async Task<IActionResult> EditHRSignature([FromBody] ApproveRequest request)
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(request.DocumentID);
                if (document == null)
                {
                    return NotFound("ไม่พบเอกสาร");
                }

                document.HrSignature = request.HRSignature; // อัปเดตชื่อ HR

                await _dbcontext.SaveChangesAsync(); // ✅ บันทึกลงฐานข้อมูล

                return Ok(new { message = "HR Signature updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาด: " + ex.Message);
            }
        }
        [HttpGet("GetApprovedFormsForHR")]
        public async Task<IActionResult> GetApprovedFormsForHR()
        {
            try
            {
                var approvedForms = await _dbcontext.Documents
                    .Where(d => d.Status == "approved") // ✅ ดึงเฉพาะเอกสารที่ HR อนุมัติแล้ว
                    .ToListAsync();

                return Ok(approvedForms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาด: " + ex.Message);
            }
        }


        [HttpGet("GetDocumentById/{documentId}")]
        public async Task<IActionResult> GetDocumentById(Guid documentId)
        {
            var document = await _dbcontext.Documents
                .Where(d => d.DocumentId == documentId)
                .Select(d => new
                {
                    d.DocumentId,
                    d.UserId,  // ✅ เพิ่ม UserId
                    d.Fullname,
                    d.LeaveTypeId,
                    d.Rolesid,
                    d.Reason,
                    d.Startdate,
                    d.Enddate,
                    d.Totalleave,
                    d.Contact,
                    d.Createdate,
                    d.LeavedType, // ✅ เพิ่ม LeavedType
                    d.LeavedStartdate,
                    d.LeavedEnddate,
                    d.Totalleaved,
                    d.FriendeContact,
                    d.Workingstart,
                    d.ApprovedDate,
                    d.HrApprovedDate,
                    d.SentToHrdate,
                    d.HrSignature,
                    d.ManagerName,
                    d.ManagerComment,
                    historyRequset = _dbcontext.Historyleaves
                        .Where(h => h.DocumentId == d.DocumentId)
                        .Select(h => new
                        {
                            h.LastTotalStickDay,
                            h.LastTotalPersonDay,
                            h.LastTotalMaternityDaystotal,
                            h.LastTotalOrdinationDays,
                            h.LastTotalVacationDays,
                            h.TotalStickDay,
                            h.TotalPersonDay,
                            h.TotalMaternityDaystotal,
                            h.TotalOrdinationDays,
                            h.TotalVacationDays,
                            h.SumStickDay,
                            h.SumPersonDay,
                            h.SumMaternityDaystotal,
                            h.SumOrdinationDays,
                            h.SumVacationDays
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return NotFound("❌ ไม่พบเอกสาร");
            }

            return Ok(document);
        }
        [HttpPut("UpdateApprovedForm")]
        public async Task<IActionResult> UpdateApprovedForm(ApproveRequest updatedForm)
        {
            if (updatedForm == null || updatedForm.DocumentID == Guid.Empty)
            {
                return BadRequest("ข้อมูลไม่ถูกต้อง");
            }

            var existingForm = await _dbcontext.Documents
                .FirstOrDefaultAsync(f => f.DocumentId == updatedForm.DocumentID);

            if (existingForm == null)
            {
                return NotFound("ไม่พบฟอร์มนี้ในระบบ");
            }

            // ✅ อัปเดตชื่อและคอมเมนต์ของ GM
            existingForm.ManagerName = updatedForm.ManagerName;
            existingForm.ManagerComment = updatedForm.ManagerComment;

            _dbcontext.Documents.Update(existingForm);
            await _dbcontext.SaveChangesAsync();

            return Ok(new { message = "อัปเดตข้อมูลสำเร็จ" });
        }

        [HttpGet("GetDocumentWithHistory/{documentId}")]
        public async Task<IActionResult> GetDocumentWithHistory(Guid documentId)
        {
            var documentWithHistory = await _dbcontext.Documents
                .Where(d => d.DocumentId == documentId)
                .Select(d => new
                {
                    d.DocumentId,
                    d.UserId,
                    d.Fullname,
                    d.LeaveTypeId,
                    d.Rolesid,
                    d.Reason,
                    d.Startdate,
                    d.Enddate,
                    d.Totalleave,
                    d.Contact,
                    d.Createdate,
                    d.LeavedType,
                    d.LeavedStartdate,
                    d.LeavedEnddate,
                    d.Totalleaved,
                    d.FriendeContact,
                    d.Workingstart,
                    d.ApprovedDate,
                    d.HrApprovedDate,
                    d.SentToHrdate,
                    d.HrSignature,
                    d.ManagerName,
                    d.ManagerComment,

                    // 🔹 รวมข้อมูลจากตาราง Historyleave
                    Historyleave = _dbcontext.Historyleaves
                        .Where(h => h.DocumentId == d.DocumentId)
                        .Select(h => new
                        {
                            h.Historyleaveid,
                            h.LastTotalStickDay,
                            h.LastTotalPersonDay,
                            h.LastTotalMaternityDaystotal,
                            h.LastTotalOrdinationDays,
                            h.LastTotalVacationDays,
                            h.TotalStickDay,
                            h.TotalPersonDay,
                            h.TotalMaternityDaystotal,
                            h.TotalOrdinationDays,
                            h.TotalVacationDays,
                            h.SumStickDay,
                            h.SumPersonDay,
                            h.SumMaternityDaystotal,
                            h.SumOrdinationDays,
                            h.SumVacationDays
                        })
                        .FirstOrDefault() // ✅ เอาข้อมูลล่าสุด
                })
                .FirstOrDefaultAsync();

            if (documentWithHistory == null)
            {
                return NotFound("❌ ไม่พบข้อมูลเอกสารหรือประวัติการลา");
            }

            return Ok(documentWithHistory);
        }


        // ✅ ลบเอกสาร
        [HttpDelete("DeleteDocument/{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId) // เปลี่ยนจาก int เป็น Guid
        {
            try
            {
                var document = await _dbcontext.Documents.FindAsync(documentId);
                if (document == null)
                {
                    return NotFound("ไม่พบเอกสารที่ต้องการลบ");
                }

                _dbcontext.Documents.Remove(document);
                await _dbcontext.SaveChangesAsync();

                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

    }


}
