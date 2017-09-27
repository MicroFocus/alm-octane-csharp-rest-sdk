using System;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Represents the idnetifier of an entity.
    /// </summary>
    /// <remarks>
    /// Octane used to use <see cref="long"/> as type of entity ID but since change to use <see cref="string"/>.
    /// This class allow backward compatability with code that use <see cref="long"/> while allowing to transition to <see cref="string"/>.
    /// </remarks>
    public sealed class EntityId
    {
        private string value;

        public EntityId()
        {
        }

        public EntityId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                // null never equals to another instance.
                return false;
            }

            if (GetType() == obj.GetType())
            {
                return value.Equals(((EntityId)obj).value);
            }
            else if (obj.GetType() == typeof(string))
            {
                return value.Equals(obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static bool operator ==(EntityId a, EntityId b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                // both null which means they are equals.
                return true;
            }

            if (ReferenceEquals(a, null))
            {
                // a is null while b not so they are not requals.
                return false;
            }

            EntityId aa = (EntityId)a;
            return aa.Equals(b);
        }

        public static bool operator !=(EntityId a, EntityId b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return value;
        }

        /// <summary>
        /// Convert EntityId to string.
        /// </summary>
        /// <param name="entityId"></param>
        public static implicit operator string(EntityId entityId)
        {
            return entityId.value;
        }

        /// <summary>
        /// Convert string to EntityId.
        /// </summary>
        /// <param name="entityId"></param>
        public static implicit operator EntityId(string entityId)
        {
            return new EntityId(entityId);
        }

        /// <summary>
        /// Convert EntityId to long while issuing obsolete warning.
        /// </summary>
        /// <param name="entityId"></param>
        [Obsolete("Octane is now using string as EntityId. In the next version of this SDK it will not be possible to using long id. Please change your code to use string instead of long.")]
        public static implicit operator long(EntityId entityId)
        {
            return Convert.ToInt64(entityId.value);
        }

        /// <summary>
        /// Convert long to EntityId while issuing obsolete warning.
        /// </summary>
        /// <param name="entityId"></param>
        [Obsolete("Octane is now using string as EntityId. In the next version of this SDK it will not be possible to using long id. Please change your code to use string instead of long.")]
        public static implicit operator EntityId(long entityId)
        {
            return new EntityId(entityId.ToString());
        }
    }
}
