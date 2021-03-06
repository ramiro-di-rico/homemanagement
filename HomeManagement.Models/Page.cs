﻿using System.Runtime.Serialization;

namespace HomeManagement.Models
{
    public class Page
    {
        [DataMember(Name = "currentPage")]
        public int CurrentPage { get; set; }

        [DataMember(Name = "pageCount")]
        public int PageCount { get; set; }

        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }

        public string Property { get; set; }

        public string FilterValue { get; set; }

        public int Operator { get; set; }
    }
}