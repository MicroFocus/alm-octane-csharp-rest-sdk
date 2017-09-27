// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Milestone entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Milestone : BaseEntity
    {
        public static string DATE_FIELD = "date";
        public static string DESCRIPTION_FIELD = "description";
        public static string RELEASES_FIELD = "releases";

        public Milestone()
        {

        }

        public Milestone(EntityId id)
            : base(id)
        {
        }

        public DateTime Date
        {
            get
            {
                return GetDateTimeValue(DATE_FIELD).Value;
            }
            set
            {
                SetDateTimeValue(DATE_FIELD, value);
            }

        }

        public String Description
        {
            get
            {
                return GetStringValue(DESCRIPTION_FIELD);
            }
            set
            {
                SetValue(DESCRIPTION_FIELD, value);
            }

        }

        public void SetRelease(EntityList<Release> releases)
        {
            SetValue(RELEASES_FIELD, releases);

        }

        public EntityList<BaseEntity> Releases
        {
            get
            {
                return (EntityList<BaseEntity>)GetValue(RELEASES_FIELD);
            }
            set
            {
                SetValue(RELEASES_FIELD, value);
            }

        }

        public DateTime GetStartDate()
        {
            DateTime date = Date;
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            return startDate;
        }

        public DateTime GetEndDate()
        {
            DateTime endDate = GetStartDate().AddDays(1);
            return endDate;
        }
    }
}
