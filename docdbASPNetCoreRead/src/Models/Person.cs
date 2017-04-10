using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Working.Models
{
    public class Person
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string l { get; set; }

        public string f { get; set; }
        public string a { get; set; }
        public override string ToString()
        {
                return JsonConvert.SerializeObject(this);
        }
    }
}