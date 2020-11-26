using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;

namespace Impostor.Plugins.Example.Commands
{
    [Command]
    class CSkinChange : ICommand
    {
        Random random = new Random();
        public string Names => "outfit";

        public string Discription => "This Command Changes Your Outfit randomly";

        public void excute(IClientPlayer player, string[] param)
        {
            var outfit = new Outfit((HatType)random.Next(94) , (ColorType)random.Next(12), (SkinType)random.Next(16), (PetType)random.Next(11));
            outfit.DressUpAsync(player);
        }
    }
}
