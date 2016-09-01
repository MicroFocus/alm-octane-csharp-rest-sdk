using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hpe.Nga.Api.Core.Entities;

namespace Hpe.Nga.Api.Core.Services
{
    /// <summary>
    /// List that used for POST/PUT requests
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityList<T> where T : BaseEntity
    {
        public EntityList()
        {
            data = new List<T>();
        }

        public EntityList(T entity)
        {
            data = new List<T>();
            data.Add(entity);
        }

        public static EntityList<T> Create(T entity)
        {
            return new EntityList<T>(entity);
        }

        public List<T> data { get; set; }
    }
}
