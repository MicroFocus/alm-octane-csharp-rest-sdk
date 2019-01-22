using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Version
{
    public class OctaneVersion : IComparable<OctaneVersion>
    {

        public static readonly OctaneVersion FENER_P3 = new OctaneVersion("12.55.7");
        public static readonly OctaneVersion INTER_P2 = new OctaneVersion("12.60.14");

        private string almVersion { get; set; }
        private int octaneVersion { get; set; }
        private int buildNumber;

        public OctaneVersion(String versionString)
        {
            string[] components = versionString.Split('.');

            if(components.Length != 3 && components.Length != 4)
            {
                throw new Exception("Unable to parse octane version from string: " + versionString);
            }

            // The first two parts are from ALM version
            almVersion = components[0] + "." + components[1];

            try
            {
                //Even though we don't use it, still good to test
                Int32.Parse(components[0]);
                Int32.Parse(components[1]);

                octaneVersion = Int32.Parse(components[2]);

                if (components.Length == 4)
                {
                    buildNumber = Int32.Parse(components[3]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse octane version from string: " + versionString, ex);
            }
        }

       
        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || this.GetType() != o.GetType()) return false;

            OctaneVersion that = (OctaneVersion)o;

            if (almVersion != null ? !almVersion.Equals(that.almVersion) : that.almVersion != null)
                return false;
            if (!octaneVersion.Equals(that.octaneVersion))
                return false;
            return buildNumber.Equals(that.buildNumber);
        }

        
        public override int GetHashCode()
        {
            int result = almVersion != null ? almVersion.GetHashCode() : 0;
            result = 31 * result +  octaneVersion.GetHashCode();
            result = 31 * result + buildNumber.GetHashCode();
            return result;
        }

        
        public int CompareTo(OctaneVersion that)
        {
            if (this == that) return 0;

            int compareAlmVersion = almVersion.CompareTo(that.almVersion);
            if (compareAlmVersion != 0)
            {
                return compareAlmVersion;
            }

            int compareOctaneVersion = octaneVersion.CompareTo(that.octaneVersion);
            if (compareOctaneVersion != 0)
            {
                return compareOctaneVersion;
            }

            return buildNumber.CompareTo(that.buildNumber);
        }


        public enum Operation
        {
            LOWER, LOWER_EQ, EQ, HIGHER_EQ, HIGHER
        }

        public static bool isBetween(OctaneVersion version, OctaneVersion lowerLimit, OctaneVersion upperLimit)
        {
            return isBetween(version, lowerLimit, upperLimit, true);
        }

        public static bool isBetween(OctaneVersion version, OctaneVersion lowerLimit, OctaneVersion upperLimit, bool inclusive)
        {
            int comparisonWithLower = version.CompareTo(lowerLimit);
            int comparisonWithUpper = version.CompareTo(upperLimit);

            //check if the version is either of the limits if the comparison is inclusive
            if ((comparisonWithLower == 0 || comparisonWithUpper == 0) && inclusive)
                return true;

            if (comparisonWithLower >= 0 && comparisonWithUpper <= 0) return true;

            return false;
        }

        public static bool compare(OctaneVersion v1, Operation op, OctaneVersion v2)
        {
            int comparison = v1.CompareTo(v2);
            if (comparison < 0 && Operation.LOWER == op) return true;
            if (comparison <= 0 && Operation.LOWER_EQ == op) return true;
            if (comparison == 0 && Operation.EQ == op) return true;
            if (comparison >= 0 && Operation.HIGHER_EQ == op) return true;
            if (comparison > 0 && Operation.HIGHER == op) return true;
            return false;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(almVersion);
            sb.Append(".");
            sb.Append(octaneVersion);
            if (buildNumber != 0)
            {
                sb.Append(".");
                sb.Append(buildNumber);
            }
            return sb.ToString();
        }

        public String getVersionString()
        {
            return almVersion + "." + octaneVersion;
        }

        

        public void discardBuildNumber()
        {
            buildNumber = 0;
        }
    }
}
