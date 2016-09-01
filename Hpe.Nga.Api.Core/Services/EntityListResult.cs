using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hpe.Nga.Api.Core.Entities;


namespace Hpe.Nga.Api.Core.Services
{
    /// <summary>
    /// List that returned by response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityListResult<T> where T : BaseEntity
    {
        public List<T> data { get; set; }

        public int? total_count { get; set; }
    }
}
