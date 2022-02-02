using System.Numerics;
using ImGuiNET;
using Vortice.Mathematics;

namespace Editor
{
    public class GuiApp : IDisposable
    {
        public Color4 ClearColor = new Color4(0.2f, 0.2f, 0.4f, 1.0f);

        List<Dock> _docks = new List<Dock>();

        public GuiApp()
        {
            ImGui.CreateContext();
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            var range = io.Fonts.GetGlyphRangesJapanese();
            io.Fonts.AddFontFromFileTTF("C:/Windows/Fonts/msgothic.ttc", 18, default, range);
            io.Fonts.Build();

            InitDocks();
        }

        public void Dispose()
        {
            ImGui.DestroyContext();
        }

        void InitDocks()
        {
            _docks.Add(new Dock("demo", "", (ref bool p_open) => { ImGui.ShowDemoWindow(ref p_open); }));
            _docks.Add(new Dock("metrics", "", (ref bool p_open) => { ImGui.ShowMetricsWindow(ref p_open); }));

            _docks.Add(new Dock("hello", "", (ref bool p_open) =>
            {
                if (p_open)
                {
                    if (ImGui.Begin("hello", ref p_open))
                    {
                        var vec4 = ClearColor.ToVector4();
                        ImGui.ColorPicker4("clear color", ref vec4);
                        ClearColor = new Color4(vec4.X, vec4.Y, vec4.Z, vec4.W);
                    }
                    ImGui.End();
                }
            }));
        }

        public void Update()
        {
            ImGui.NewFrame();
            DockSpace("__dockspace__");

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Window"))
                {
                    foreach (var dock in _docks)
                    {
                        // Dockの表示状態と chekmark を連動
                        ImGui.MenuItem(dock.MenuLabel, dock.MenuShortCut, ref dock.IsOpen);
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            foreach (var dock in _docks)
            {
                // dock の描画
                dock.Draw();
            }
            ImGui.Render();
        }

        void DockSpace(string name)
        {
            var flags = (ImGuiWindowFlags.MenuBar
                     | ImGuiWindowFlags.NoDocking
                     | ImGuiWindowFlags.NoBackground
                     | ImGuiWindowFlags.NoTitleBar
                     | ImGuiWindowFlags.NoCollapse
                     | ImGuiWindowFlags.NoResize
                     | ImGuiWindowFlags.NoMove
                     | ImGuiWindowFlags.NoBringToFrontOnFocus
                     | ImGuiWindowFlags.NoNavFocus
                     );

            var viewport = ImGui.GetMainViewport();
            var pos = viewport.Pos;
            var size = viewport.Size;
            ImGui.SetNextWindowPos(pos);
            ImGui.SetNextWindowSize(size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            // DockSpace
            ImGui.Begin(name, flags);
            ImGui.PopStyleVar(3);
            var dockspace_id = ImGui.GetID(name);
            ImGui.DockSpace(dockspace_id, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);
            ImGui.End();
        }

        public void LoadPath(string path)
        {
            var scene = JsonScene.Scene.LoadPath(path);
            if (scene == null)
            {
                return;
            }
            var name = $"json###{Path.GetFileName(path)}";
            var tree = new JsonTree(name, scene.Gltf);
            _docks.Add(new Dock(name, "", tree.Show));
        }
    }
}
