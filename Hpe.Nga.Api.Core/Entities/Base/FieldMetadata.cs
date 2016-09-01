using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.Attributes;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Field Metadata entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary
    [CustomCollectionPathAttribute("metadata/fields")]
    public class FieldMetadata : BaseEntity
    {
        public static string ENTITY_NAME_FIELD = "entity_name";

    }
}
