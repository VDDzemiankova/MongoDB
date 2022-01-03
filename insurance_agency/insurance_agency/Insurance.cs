using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace insurance_agency
{
    class Insurance
    {

        [BsonId]
        public int Id { get; set; }
        public Insured Insured { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public Employee Employee { get; set; }
    }
    class Insured 
    {
        public string Name { get; set; }
        public string Adress { get; set; }
    }
    class Employee 
    { 
        public string Name { get; set; }
        [BsonIgnoreIfNull]
        public string Adress { get; set; }
        [BsonIgnoreIfDefault]
        public int Experience { get; set; }
    }
}
