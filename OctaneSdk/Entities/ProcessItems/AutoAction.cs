using MicroFocus.Adm.Octane.Api.Core.Entities.ProcessItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class AutoAction : ProcessItem
    {
        public AutoAction() : base() {
        }
        public AutoAction(EntityId id) : base(id) {
        }
    }
}
