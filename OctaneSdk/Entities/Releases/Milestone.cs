/*
 * Copyright 2016-2023 Open Text.
 *
 * The only warranties for products and services of Open Text and
 * its affiliates and licensors (“Open Text”) are as may be set forth
 * in the express warranty statements accompanying such products and services.
 * Nothing herein should be construed as constituting an additional warranty.
 * Open Text shall not be liable for technical or editorial errors or
 * omissions contained herein. The information contained herein is subject
 * to change without notice.
 *
 * Except as specifically indicated otherwise, this document contains
 * confidential information and a valid license is required for possession,
 * use or copying. If this work is provided to the U.S. Government,
 * consistent with FAR 12.211 and 12.212, Commercial Computer Software,
 * Computer Software Documentation, and Technical Data for Commercial Items are
 * licensed to the U.S. Government under vendor's standard commercial license.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *   http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using MicroFocus.Adm.Octane.Api.Core.Services;
using System;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Milestone entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Milestone : BaseEntity
    {
        public static string DATE_FIELD = "date";
        public static string DESCRIPTION_FIELD = "description";
        public static string RELEASE_FIELD = "release";

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

        public string Description
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

        public void SetRelease(Release release)
        {
            SetValue(RELEASE_FIELD, release);

        }

        public Release Release
        {
            get
            {
                return (Release)GetValue(RELEASE_FIELD);
            }
            set
            {
                SetValue(RELEASE_FIELD, value);
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
