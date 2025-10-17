using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLCVan
{
    public class PermissionHelper
    {
        public static bool HasPermission(string MaQuyen)
        {
            var list = HttpContext.Current.Session["ListQuyen"] as List<string>;
            if (list == null) return false;
            return list.Contains(MaQuyen);
        }
    }
}