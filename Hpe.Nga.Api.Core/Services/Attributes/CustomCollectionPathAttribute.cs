using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.Attributes
{

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomCollectionPathAttribute : Attribute
    {

        public string Path {get; private set;}            

        public CustomCollectionPathAttribute(string path)
        {
            this.Path = path;
        }


    }
}
