using System;
using System.Collections.Generic;

namespace Minsky.Entities.Integration.Sneaker
{
    [Serializable]
    public sealed class ServerInfoContract
    {
        public string name { get; set; }
        public List<PlayerContract> players { get; set; } = new List<PlayerContract>();
    }
}
