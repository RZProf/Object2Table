using System.Collections.Generic;

namespace Object2Table.UnitTests.Models
{
    public class PersonWithMetas : SimplePerson
    {
        public IEnumerable<Dictionary<string, object>> Metas { get; set; }
    }
}