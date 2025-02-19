using System;
using System.Globalization;

namespace TheExpertise_Emp.DTO
{
    public static class helperFucntion
    {
        public static DateTime? ConvertStrToDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) // เช็คค่าว่างด้วย
            {
                return null; // คืนค่า null ถ้าไม่มีค่า ไม่ต้องโยน Exception
            }

         
            DateTime? dateTime;
            dateTime = DateTime.ParseExact(dateStr.Replace("-", "/"), "yyyy/MM/dd", CultureInfo.InvariantCulture);
            dateTime = dateTime.Value.Date.Add(DateTime.Now.TimeOfDay);
            return dateTime;
        }
    }
}
