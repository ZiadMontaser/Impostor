using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Inner;

namespace Impostor.Plugins.Example
{
    public class Outfit
    {
        private readonly HatType _hat;
        private readonly ColorType _color;
        private readonly SkinType _skin;
        private readonly PetType _pet;

        public Outfit(HatType hat,ColorType color,SkinType skin,PetType pet)
        {
            _hat =hat;
            _color = color;
            _skin = skin;
            _pet = pet;
        }

        public Outfit(IInnerPlayerInfo info)
        {
            _hat = (HatType) info.HatId;
            _color = (ColorType) info.ColorId;
            _skin =(SkinType) info.SkinId;
            _pet = (PetType) info.PetId;
        }

        public async ValueTask DressUpAsync(IClientPlayer player)
        {
            await player.Character.SetHatAsync(_hat);
            await player.Character.SetColorAsync(_color);
            await player.Character.SetSkinAsync(_skin);
            await player.Character.SetPetAsync(_pet); 
        }
    }
}
