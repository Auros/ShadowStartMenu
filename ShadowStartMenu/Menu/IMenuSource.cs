using System.Collections.Generic;

namespace ShadowStartMenu.Menu
{
    public interface IMenuSource
    {
        IEnumerable<IApp> Apps { get; }
        void Add(IApp app, params string[] subRoots);
        void Remove(IApp app);
    }
}