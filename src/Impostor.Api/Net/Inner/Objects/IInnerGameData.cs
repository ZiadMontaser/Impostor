using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerGameData : IInnerNetObject
    {
        public ValueTask UpdateGameDataAsync(int? targetClientId = null);
    }
}
