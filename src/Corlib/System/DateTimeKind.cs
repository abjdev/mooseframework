﻿//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System
{

    // This enum is used to indentify DateTime instances in cases when they are known to be in local time, 
    // UTC time or if this information has not been specified or is not applicable.
    public enum DateTimeKind
    {
        Unspecified = 0,
        Utc = 1,
        Local = 2,
    }
}