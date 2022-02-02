using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;


namespace Editor
{
    class Device : IDisposable
    {
        ID3D11Device _device;
        ID3D11DeviceContext _deviceContext;
        IDXGISwapChain? _swapChain;
        // ID3D11Texture2D backBuffer;
        // ID3D11RenderTargetView renderView;

        public Device()
        {
            var flags = DeviceCreationFlags.BgraSupport; // for D2D
#if DEBUG
            flags |= DeviceCreationFlags.Debug;
#endif
            D3D11.D3D11CreateDevice(null, DriverType.Hardware, DeviceCreationFlags.None, null, out _device, out _deviceContext).CheckError();
        }

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

        // }

        SwapChainDescription swapchainDesc;
        public void Render(IntPtr hwnd, int w, int h)
        {
            if (_swapChain == null)
            {
                var dxgiFactory = _device.QueryInterface<IDXGIDevice>().GetParent<IDXGIAdapter>().GetParent<IDXGIFactory>();
                swapchainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    BufferDescription = new ModeDescription(w, h, Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputWindow = hwnd,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Vortice.DXGI.Usage.RenderTargetOutput
                };
                _swapChain = dxgiFactory.CreateSwapChain(_device, swapchainDesc);
                dxgiFactory.MakeWindowAssociation(hwnd, WindowAssociationFlags.IgnoreAll);
            }
            else if (swapchainDesc.BufferDescription.Width != w || swapchainDesc.BufferDescription.Height != h)
            {
                _swapChain.ResizeBuffers(1, w, h, swapchainDesc.BufferDescription.Format, swapchainDesc.Flags);
            }

            _swapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            if (_swapChain != null)
            {
                _swapChain.Dispose();
                _swapChain = null;
            }
            if (_deviceContext != null)
            {
                _deviceContext.Dispose();
            }
            if (_device != null)
            {
                _device.Dispose();
            }
        }
    }
}
