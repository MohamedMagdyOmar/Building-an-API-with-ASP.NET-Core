using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    // this class represent smaller or different version of what the camp is
    public class CampModel
    {
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public int Length { get; set; } = 1;

        // using the name of the class entity "Location" before the name(prefix) of the property, makes Automapper understand that should 
        // fill this data from Location Object "nasa7a menno"
        // the problem with this approach that the retrieved json will have same variable name shich is not looks good
        // so we will use another way of mapping as it appears in "Venue" Property
        public string Venue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }
    }
}
