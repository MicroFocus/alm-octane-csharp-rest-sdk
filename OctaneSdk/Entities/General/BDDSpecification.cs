using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities.General
{
    public class BDDSpecification : BaseEntity
    {
        public static string TYPE_BDDSPEC = "bdd_spec";

        public BDDSpecification()
            : base()
        {
        }

        public BDDSpecification(EntityId id)
            : base(id)
        {
        }

    }
}
