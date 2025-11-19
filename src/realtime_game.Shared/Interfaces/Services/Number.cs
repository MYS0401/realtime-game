using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realtime_game.Shared.Interfaces.Services
{

    [MessagePackObject]
    public class Number
    {
        [MessagePack.Key(0)]
        public float x;
        [MessagePack.Key(1)]
        public float y;
        [MessagePack.Key(2)]
        public float z;
    }
}
