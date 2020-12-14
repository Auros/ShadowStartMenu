using System.Collections.Generic;

namespace ShadowStartMenu.Menu
{
    public interface IMenuSource
    {
        /// <summary>
        /// A collection of apps.
        /// </summary>
        IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Add an app to this source.
        /// </summary>
        /// <param name="app">The app to add.</param>
        /// <param name="shortcutType">The type of the shortcut.</param>
        /// <param name="subRoots">Folder subroots.</param>
        void Add(IApp app, ShortcutType shortcutType, params string[] subRoots);

        /// <summary>
        /// Removes an app from this source.
        /// </summary>
        /// <param name="app">The app to remove.</param>
        void Remove(IApp app);
    }
}