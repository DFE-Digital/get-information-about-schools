﻿using Edubase.Data.Entity;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Admin
{
    public class LogMessagesViewModel
    {
        public IEnumerable<LogMessage> Messages { get; set; }
        public string SkipToken { get; set; }
        public string DateFilter { get; set; }

        public LogMessagesViewModel()
        {

        }
        

        public LogMessagesViewModel(LogMessagesDto dto) 
        {
            Messages = dto.Items;
            SkipToken = dto.SkipToken;
        }
    }
}