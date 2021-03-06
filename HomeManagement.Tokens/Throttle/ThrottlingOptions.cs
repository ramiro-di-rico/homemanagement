﻿namespace HomeManagement.Api.Core.Throttle
{
    public class ThrottlingOptions
    {
        public int MaxRequestsAllowed { get; set; }

        public static ThrottlingOptions GetDefaultOptions() => new ThrottlingOptions { MaxRequestsAllowed = 200 };
    }
}