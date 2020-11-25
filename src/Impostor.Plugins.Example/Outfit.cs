using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
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
            using(var w = MessageWriter.Get(MessageType.Reliable))
            {
                w.StartMessage(MessageFlags.GameData);
                w.Write(player.Game.Code);

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(player.Character.NetId);
                w.Write((byte)RpcCalls.SetHat);
                w.WritePacked((uint)_hat);
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(player.Character.NetId);
                w.Write((byte)RpcCalls.SetColor);
                w.WritePacked((uint)_color);
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(player.Character.NetId);
                w.Write((byte)RpcCalls.SetSkin);
                w.WritePacked((uint)_skin);
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(player.Character.NetId);
                w.Write((byte)RpcCalls.SetPet);
                w.WritePacked((uint)_pet);
                w.EndMessage();

                w.EndMessage();

                await player.Game.SendToAllAsync(w);
            }
        }
    }
}
