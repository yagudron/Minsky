using System;

namespace Minsky.Entities.Integration.Sneaker
{
    [Serializable]
    public sealed class PlayerContract
    {
        public string name { get; set; }
        public string type { get; set; }
    }
}
