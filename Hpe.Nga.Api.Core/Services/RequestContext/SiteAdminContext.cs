using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.RequestContext
{
    /// <summary>
    /// Define URL for admin context
    /// </summary>
    public class SiteAdminContext : IRequestContext
    {

        public string GetPath()
        {
            return "/admin";
        }
    }
}
