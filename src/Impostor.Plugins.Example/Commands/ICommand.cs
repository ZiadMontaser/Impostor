using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Net;

namespace Impostor.Plugins.Example.Commands
{
    interface ICommand
    {
        string Names { get; }
        string Discription { get; }
        void excute(IClientPlayer player, string[] param);
    }
}
