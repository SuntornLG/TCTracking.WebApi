

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace TCTracking.Service.Dtos
{
    public class TCSRequest
    {
        public string TCNumber { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Oldpart { get; set; }
        public string Newpart { get; set; }
        public string Partprocess { get; set; }
        public string Assymanual { get; set; }
        public string Changetype { get; set; }
        public string RM { get; set; }
        public string PM { get; set; }
        public string Effectivelot { get; set; }

        public string DispoCode { get; set; }
        public bool Exchangeable { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public int Qty { get; set; }

        public List<string> Images { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
