using System;

namespace Impostor.Api.Innersloth
{
    [Flags]
    public enum CharacterStates
    {
        None = 0,
        Disconnected = 1,
        Impostor = 2,
        Dead = 4,
    }
}
