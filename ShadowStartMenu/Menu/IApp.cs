namespace ShadowStartMenu.Menu
{
    public interface IApp
    {
        /// <summary>
        /// The name of the app.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The folder path of the app.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The shortcut path to this app.
        /// </summary>
        string? ShortcutPath { get; set; }
    }
}