using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class ProcessQualityGate : Process
    {
        public ProcessQualityGate(IDictionary<string, object> properties)
        : base(properties)
        {
        }
        public ProcessQualityGate() : base()
        {
        }
        public ProcessQualityGate(EntityId id) : base(id) {
        }
    }

}
