using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace first_test_task
{
    public class Medication
    {
        public string Name;
        public string Dosage;
        public string Number;
        public string Fabricator;
        public string Country;

        public Medication() { }
        public Medication(string name, string dosage, string number , string fabricator, string country) { 
            Name = name;
            Dosage = dosage;
            Number = number;
            Fabricator = fabricator;
            Country = country;
        }

    }
}
