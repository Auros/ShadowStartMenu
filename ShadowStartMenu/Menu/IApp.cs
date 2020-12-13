namespace ShadowStartMenu.Menu
{
    public interface IApp
    {
        string Name { get; }
        string Path { get; }
        string? ShortcutPath { get; set; }
    }
}