using Impostor.Api.Net;

namespace ZirnoPlugin.Commands
{
    interface ICommand
    {
        string Names { get; }
        string Discription { get; }
        void excute(IClientPlayer player, string[] param);
    }
}
