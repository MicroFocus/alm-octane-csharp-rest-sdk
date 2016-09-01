using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Test entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Test : BaseEntity
    {
        public static string SUBTYPE_FIELD = "subtype";
        public static string PHASE_FIELD = "phase";


        public static string SUBTYPE_MANUAL_TEST = "test_manual";

        public Test()
        {
        }

        public Test(long id)
            : base(id)
        {
        }

        public string SubType
        {
            get
            {
                return GetStringValue(SUBTYPE_FIELD);
            }
            set
            {
                SetValue(SUBTYPE_FIELD, value);
            }

        }

        public Phase Phase
        {
            get
            {
                return (Phase)GetValue(PHASE_FIELD);
            }
            set
            {
                SetValue(PHASE_FIELD, value);
            }
        }


    }
}
