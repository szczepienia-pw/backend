﻿using System.Text.Json.Serialization;
using backend.Helpers;
using backend.Models.Accounts.AdditionalData;

namespace backend.Models.Accounts
{
    public class PatientModel : AccountModel
    {
        public string Pesel { get; set; }
        public virtual AddressModel Address { get; set; }

        // for the purposes of seeding
        [JsonIgnore]
        public int AddressId { get; set; }

        public override AccountTypeEnum GetEnum()
        {
            return AccountTypeEnum.Patient;
        }
    }
}
