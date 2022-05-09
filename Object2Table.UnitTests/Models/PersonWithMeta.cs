using System.Collections.Generic;

namespace Object2Table.UnitTests.Models
{
    public class PersonWithMeta : SimplePerson
    {
        public Dictionary<string, object> Meta { get; set; }
    }
}