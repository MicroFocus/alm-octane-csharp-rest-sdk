using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class ModelBasedTest: Test
    {
            public const string SUBTYPE_MODEL_BASED_TEST = "model_based_test";

            public ModelBasedTest()
                : base()
            {
            }

            public ModelBasedTest(EntityId id)
                : base(id)
            {
            }
    }
}
