using System;
using System.Collections.Generic;
using System.Text;

namespace Impostor.Api.Innersloth
{
    public enum CharacterStates : byte
    {
        Disconnected = 1,
        Impostor = 2,
        Dead = 4,
    }
}
