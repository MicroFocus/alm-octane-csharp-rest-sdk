using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.RequestContext
{
    /// <summary>
    /// Base class for request contexts.
    /// The main purpose of this class and its hierarchy is to allow to define different URL path according to different context.
    /// </summary>
    public interface IRequestContext
    {
        String GetPath();
    }
}
