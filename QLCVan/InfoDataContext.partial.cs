using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace QLCVan
{
    public partial class InfoDataContext 
    {
        public InfoDataContext() : base(ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString2"].ConnectionString)
        {
            OnCreated();
        }
    }
}