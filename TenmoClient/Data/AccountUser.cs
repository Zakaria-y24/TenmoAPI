﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class AccountUser
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
}