using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public static class ProcessFactory
    {
        public static Process Create(IDictionary<string, object> properties)
        {
            string subtype = null;

            object raw;
            if (properties != null && properties.TryGetValue(Process.SUBTYPE_FIELD, out raw) && raw != null)
                subtype = raw.ToString();

            if (subtype == Process.SUBTYPE_MANUAL_ACTION)
                return new ProcessAction(properties);

            if (subtype == Process.SUBTYPE_AUTO_ACTION)
                return new ProcessAutoAction(properties);

            if (subtype == Process.SUBTYPE_QUALITY_GATE)
                return new ProcessQualityGate(properties);

            return new Process(properties);
        }

    }
}

