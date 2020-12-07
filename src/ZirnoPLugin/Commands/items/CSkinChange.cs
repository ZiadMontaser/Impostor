using System;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using ZirnoPlugin.Viresed;

namespace ZirnoPlugin.Commands
{
    [Command]
    class CSC : CSkinChange
    {
        public override string Names => "of";
    }

    [Command]
    class CSkinChange : ICommand
    {
        Random random = new Random();
        public virtual string Names => "outfit";

        public string Discription => "This Command Changes Your Outfit randomly";

        public void excute(IClientPlayer player, string[] param)
        {
            var outfit = new Outfit((HatType)random.Next(94) , (ColorType)random.Next(12), (SkinType)random.Next(16), (PetType)random.Next(11));
            outfit.DressUpAsync(player);
        }
    }
}
