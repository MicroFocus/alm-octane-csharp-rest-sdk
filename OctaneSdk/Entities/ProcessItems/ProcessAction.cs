using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class ProcessAction : Process
    {
        public ProcessAction(IDictionary<string, object> properties)
        : base(properties)
        {
        }
        public ProcessAction() : base()
        {
        }
        public ProcessAction(EntityId id) : base(id) {
        }
    }
}
