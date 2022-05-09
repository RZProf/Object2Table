using System;

namespace Object2Table
{
    internal class PropertyEmitter
    {
        internal string Property { get; set; }

        internal Delegate Emitter { get; set; }
    }
}