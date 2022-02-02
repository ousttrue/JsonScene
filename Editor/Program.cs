using Vortice.Win32;
using System.Runtime.CompilerServices;
using Vortice.Mathematics;
using ImGuiNET;
using VorticeImGui;

namespace Editor
{

    static class Program
    {
        const uint PM_REMOVE = 1;
        const string CLASS_NAME = "WndClass";
        const string WINDOW_NAME = "ImGui";
        const int WIDTH = 800;
        const int HEIGHT = 600;

        delegate bool ProcessMessage(WindowMessage msg, UIntPtr wParam, IntPtr lParam);
        static Dictionary<IntPtr, ProcessMessage> s_windows = new Dictionary<IntPtr, ProcessMessage>();

        [STAThread]
        static void Main()
        {
#nullable disable
            var moduleHandle = Kernel32.GetModuleHandle(null);
#nullable enable

            var wndClass = new WNDCLASSEX
            {
                Size = Unsafe.SizeOf<WNDCLASSEX>(),
                Styles = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW | WindowClassStyles.CS_OWNDC,
                WindowProc = WndProc,
                InstanceHandle = moduleHandle,
                CursorHandle = User32.LoadCursor(IntPtr.Zero, SystemCursor.IDC_ARROW),
                BackgroundBrushHandle = IntPtr.Zero,
                IconHandle = IntPtr.Zero,
                ClassName = CLASS_NAME,
            };
            if (User32.RegisterClassEx(ref wndClass) == 0)
            {
                throw new Exception();
            }

            var screenWidth = User32.GetSystemMetrics(SystemMetrics.SM_CXSCREEN);
            var screenHeight = User32.GetSystemMetrics(SystemMetrics.SM_CYSCREEN);
            var x = (screenWidth - WIDTH) / 2;
            var y = (screenHeight - HEIGHT) / 2;
            var style = WindowStyles.WS_OVERLAPPEDWINDOW;
            var styleEx = WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_WINDOWEDGE;
            var windowRect = new Rect(0, 0, WIDTH, HEIGHT);
            User32.AdjustWindowRectEx(ref windowRect, style, false, styleEx);
            var windowWidth = windowRect.Right - windowRect.Left;
            var windowHeight = windowRect.Bottom - windowRect.Top;
            var hwnd = User32.CreateWindowEx(
                (int)styleEx, CLASS_NAME, WINDOW_NAME, (int)style,
                x, y, windowWidth, windowHeight,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (hwnd == IntPtr.Zero)
            {
                throw new Exception();
            }

            // imgui
            ImGui.CreateContext();
            var io = ImGui.GetIO();
            var imguiInputHandler = new ImGuiInputHandler(hwnd);

            s_windows.Add(hwnd, imguiInputHandler.ProcessMessage);
            User32.ShowWindow(hwnd, ShowWindowCommand.Normal);

            using (var renderer = new Device())
            {

                while (true)
                {
                    var isQuit = false;
                    while (User32.PeekMessage(out var msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                    {
                        User32.TranslateMessage(ref msg);
                        User32.DispatchMessage(ref msg);
                        if (msg.Value == (uint)WindowMessage.Quit)
                        {
                            isQuit = true;
                            break;
                        }
                    }
                    if (isQuit)
                    {
                        break;
                    }

                    imguiInputHandler.Update();

                    renderer.Render(hwnd, (int)io.DisplaySize.X, (int)io.DisplaySize.Y);
                }
            }
        }

        static IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            if (s_windows.TryGetValue(hWnd, out ProcessMessage processMessage))
            {
                if (processMessage((WindowMessage)msg, wParam, lParam))
                {
                    return IntPtr.Zero;
                }
            }

            switch ((WindowMessage)msg)
            {
                case WindowMessage.Destroy:
                    User32.PostQuitMessage(0);
                    break;
            }

            return User32.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}
