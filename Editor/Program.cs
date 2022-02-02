using Vortice.Win32;
using System.Runtime.CompilerServices;
using Vortice.Direct3D11;
using ImGuiNET;
using Vortice.Direct3D;
using System.Diagnostics;
using System.Numerics;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace VorticeImGui
{
    class App
    {
        // public Win32Window Win32Window;

        // ID3D11Device device;
        // ID3D11DeviceContext deviceContext;
        // IDXGISwapChain swapChain;
        // ID3D11Texture2D backBuffer;
        // ID3D11RenderTargetView renderView;

        // Format format = Format.R8G8B8A8_UNorm;

        // ImGuiRenderer imGuiRenderer;
        // ImGuiInputHandler imguiInputHandler;
        // Stopwatch stopwatch = Stopwatch.StartNew();
        // TimeSpan lastFrameTime;

        // IntPtr imGuiContext;

        // public App(Win32Window win32window, ID3D11Device device, ID3D11DeviceContext deviceContext)
        // {
        //     Win32Window = win32window;
        //     this.device = device;
        //     this.deviceContext = deviceContext;

        //     imGuiContext = ImGui.CreateContext();
        //     ImGui.SetCurrentContext(imGuiContext);

        //     imGuiRenderer = new ImGuiRenderer(this.device, this.deviceContext);
        //     imguiInputHandler = new ImGuiInputHandler(Win32Window.Handle);

        //     ImGui.GetIO().DisplaySize = new Vector2(Win32Window.Width, Win32Window.Height);
        // }

        // public virtual bool ProcessMessage(uint msg, UIntPtr wParam, IntPtr lParam)
        // {
        //     ImGui.SetCurrentContext(imGuiContext);
        //     if (imguiInputHandler.ProcessMessage((WindowMessage)msg, wParam, lParam))
        //         return true;

        //     switch ((WindowMessage)msg)
        //     {
        //         case WindowMessage.Size:
        //             switch ((SizeMessage)wParam)
        //             {
        //                 case SizeMessage.SIZE_RESTORED:
        //                 case SizeMessage.SIZE_MAXIMIZED:
        //                     Win32Window.IsMinimized = false;

        //                     var lp = (int)lParam;
        //                     Win32Window.Width = Utils.Loword(lp);
        //                     Win32Window.Height = Utils.Hiword(lp);

        //                     resize();
        //                     break;
        //                 case SizeMessage.SIZE_MINIMIZED:
        //                     Win32Window.IsMinimized = true;
        //                     break;
        //                 default:
        //                     break;
        //             }
        //             break;
        //     }

        //     return false;
        // }

        // public void UpdateAndDraw()
        // {
        //     UpdateImGui();
        //     ImGui.ShowDemoWindow();
        //     render();
        // }

        // void resize()
        // {
        //     if (renderView == null)//first show
        //     {
        //         var dxgiFactory = device.QueryInterface<IDXGIDevice>().GetParent<IDXGIAdapter>().GetParent<IDXGIFactory>();

        //         var swapchainDesc = new SwapChainDescription()
        //         {
        //             BufferCount = 1,
        //             BufferDescription = new ModeDescription(Win32Window.Width, Win32Window.Height, format),
        //             IsWindowed = true,
        //             OutputWindow = Win32Window.Handle,
        //             SampleDescription = new SampleDescription(1, 0),
        //             SwapEffect = SwapEffect.Discard,
        //             Usage = Vortice.DXGI.Usage.RenderTargetOutput
        //         };

        //         swapChain = dxgiFactory.CreateSwapChain(device, swapchainDesc);
        //         dxgiFactory.MakeWindowAssociation(Win32Window.Handle, WindowAssociationFlags.IgnoreAll);

        //         backBuffer = swapChain.GetBuffer<ID3D11Texture2D>(0);
        //         renderView = device.CreateRenderTargetView(backBuffer);
        //     }
        //     else
        //     {
        //         renderView.Dispose();
        //         backBuffer.Dispose();

        //         swapChain.ResizeBuffers(1, Win32Window.Width, Win32Window.Height, format, SwapChainFlags.None);

        //         backBuffer = swapChain.GetBuffer<ID3D11Texture2D1>(0);
        //         renderView = device.CreateRenderTargetView(backBuffer);
        //     }
        // }

        // public virtual void UpdateImGui()
        // {
        //     ImGui.SetCurrentContext(imGuiContext);
        //     var io = ImGui.GetIO();

        //     var now = stopwatch.Elapsed;
        //     var delta = now - lastFrameTime;
        //     lastFrameTime = now;
        //     io.DeltaTime = (float)delta.TotalSeconds;

        //     imguiInputHandler.Update();

        //     ImGui.NewFrame();
        // }

        // void render()
        // {
        //     ImGui.Render();

        //     var dc = deviceContext;
        //     dc.ClearRenderTargetView(renderView, new Color4(0, 0, 0));
        //     dc.OMSetRenderTargets(renderView);
        //     dc.RSSetViewport(0, 0, Win32Window.Width, Win32Window.Height);

        //     imGuiRenderer.Render(ImGui.GetDrawData());

        //     swapChain.Present(0, PresentFlags.None);
        // }
    }

    class Program
    {
        const uint PM_REMOVE = 1;

        [STAThread]
        static void Main()
        {
            new Program().Run();
        }

        bool quitRequested;

        // ID3D11Device device;
        // ID3D11DeviceContext deviceContext;

        Dictionary<IntPtr, App> windows = new Dictionary<IntPtr, App>();

        void Run()
        {
            // D3D11.D3D11CreateDevice(null, DriverType.Hardware, DeviceCreationFlags.None, null, out device, out deviceContext);

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
                ClassName = "WndClass",
            };

            User32.RegisterClassEx(ref wndClass);

            var win32window = new Win32Window(wndClass.ClassName, "Vortice ImGui", 800, 600);
            // var app = new App(win32window, device, deviceContext);
            // windows.Add(win32window.Handle, app);
            User32.ShowWindow(win32window.Handle, ShowWindowCommand.Normal);

            // app.Show();

            while (true)
            {
                if (User32.PeekMessage(out var msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                    if (msg.Value == (uint)WindowMessage.Quit)
                    {
                        quitRequested = true;
                        break;
                    }
                }

                // foreach (var window in windows.Values)
                //     window.UpdateAndDraw();
            }
        }

        IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            // App window;
            // windows.TryGetValue(hWnd, out window);

            // if (window?.ProcessMessage(msg, wParam, lParam) ?? false)
            //     return IntPtr.Zero;

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
