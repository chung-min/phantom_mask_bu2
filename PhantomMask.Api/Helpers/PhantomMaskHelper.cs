using System.Text.RegularExpressions;

namespace PhantomMask.Api.Helpers
{
    public class PhantomMaskHelper
    {
        /// <summary>
        /// 判斷指定的營業時間字串中，是否在特定星期與時間內營業。
        /// </summary>
        /// <param name="openingHours">營業時間 Mon 08:00 - 18:00, Tue 13:00 - 18:00</param>
        /// <param name="day">查詢的星期縮寫（如 Mon、Tue），未填代表不指定星期</param>
        /// <param name="time">查詢的時間字串（格式為HHmm"），未填代表不指定時間</param>
        /// <returns>若指定時間與/或星期幾在營業時間範圍內，回傳 true；否則回傳 false</returns>
        public bool IsOpenAt(string openingHours, string day, string time)
        {
            try
            {
                var blocks = openingHours.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                foreach (var block in blocks)
                {
                    // openingHours格式
                    var match = Regex.Match(block, @"^(?<day>\w{3}) (?<start>\d{2}:\d{2}) - (?<end>\d{2}:\d{2})$");
                    if (!match.Success)
                    {
                        continue;
                    }

                    var dayOfWeek = match.Groups["day"].Value;
                    var startStr = match.Groups["start"].Value;
                    var endStr = match.Groups["end"].Value;

                    // 如果有指定day，則檢查是否符合
                    if (!string.IsNullOrWhiteSpace(day) && !day.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    //特殊處理 24:00 → 23:59:59，代表整天營業
                    if (TimeSpan.TryParse(startStr, out TimeSpan start))
                    {
                        start = startStr == "24:00" ? new TimeSpan(23, 59, 59) : TimeSpan.Parse(startStr);
                    }

                    if (TimeSpan.TryParse(endStr, out TimeSpan end))
                    {
                        start = startStr == "24:00" ? new TimeSpan(23, 59, 59) : TimeSpan.Parse(startStr);
                    }

                    if (!string.IsNullOrWhiteSpace(time))
                    {
                        TimeSpan.TryParse(time.Insert(2, ":"), out TimeSpan timeSpan);
                        if (timeSpan >= start && timeSpan <= end)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // 只指定day，不指定時間 → 只要有該日開店時間就算
                        if (!string.IsNullOrEmpty(day))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
