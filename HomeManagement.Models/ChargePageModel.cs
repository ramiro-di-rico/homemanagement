﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HomeManagement.Models
{
    public class ChargePageModel : Page
    {
        [DataMember(Name = "accountId")]
        public int AccountId { get; set; }

        [DataMember(Name = "charges")]
        public List<ChargeModel> Charges { get; set; }
    }
}
