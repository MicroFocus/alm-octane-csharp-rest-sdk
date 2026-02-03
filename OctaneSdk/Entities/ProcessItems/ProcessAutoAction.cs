using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class ProcessAutoAction : Process
    {
        public ProcessAutoAction(IDictionary<string, object> properties)
        : base(properties)
        {
        }
        public ProcessAutoAction() : base()
        {
        }
        public ProcessAutoAction(EntityId id) : base(id) {
        }
    }
}
