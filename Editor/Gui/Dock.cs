namespace Editor
{
    public delegate void DrawDelegate(ref bool p_open);

    public class Dock
    {
        public readonly string MenuLabel;
        public readonly string MenuShortCut;
        public readonly DrawDelegate Show;
        public bool IsOpen = true;

        public Dock(string name, string shortCut, DrawDelegate show)
        {
            MenuLabel = name;
            MenuShortCut = shortCut;
            Show = show;
        }

        public void Draw()
        {
            if (IsOpen)
            {
                Show(ref IsOpen);
            }
        }
    }
}
