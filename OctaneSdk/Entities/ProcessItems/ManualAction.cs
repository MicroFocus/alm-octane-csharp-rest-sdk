using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class ManualAction : ProcessItem
    {
        public ManualAction() : base() {
        }
        public ManualAction(EntityId id) : base(id) {
        }
    }
}
