using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;

namespace HADotNet.CommandCenter.Utils
{
    public static class AlertManager
    {
        public const string GRP_INFO = "AlertInfo";
        public const string GRP_SUCCESS = "AlertSuccess";
        public const string GRP_WARNING = "AlertWarning";
        public const string GRP_ERROR = "AlertError";

        public static void AddInfo(this ITempDataDictionary temp, string message) => AppendOrCreateList(temp, message, GRP_INFO);
        public static void AddSuccess(this ITempDataDictionary temp, string message) => AppendOrCreateList(temp, message, GRP_SUCCESS);
        public static void AddWarning(this ITempDataDictionary temp, string message) => AppendOrCreateList(temp, message, GRP_WARNING);
        public static void AddError(this ITempDataDictionary temp, string message) => AppendOrCreateList(temp, message, GRP_ERROR);

        private static void AppendOrCreateList(ITempDataDictionary temp, string alert, string group)
        {
            if (temp.Peek(group) is List<string> tempList)
            {
                tempList.Add(alert);
            }
            else
            {
                temp[group] = new List<string> { alert };
                temp.Keep(group);
            }

            temp.Save();
        }
    }
}
