using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Editor
{
    class Device : IDisposable
    {
        ID3D11Device _device;
        ID3D11DeviceContext _deviceContext;
        IDXGISwapChain? _swapChain;
        ID3D11RenderTargetView? _rtv;

        public Device()
        {
            var flags = DeviceCreationFlags.BgraSupport; // for D2D
#if DEBUG
            flags |= DeviceCreationFlags.Debug;
#endif
            D3D11.D3D11CreateDevice(null, DriverType.Hardware, DeviceCreationFlags.None, null, out _device, out _deviceContext).CheckError();
        }

        SwapChainDescription swapchainDesc;
        public void BeginFrame(IntPtr hwnd, int width, int height, Color4 clearColor)
        {
            if (_swapChain == null)
            {
                var dxgiFactory = _device.QueryInterface<IDXGIDevice>().GetParent<IDXGIAdapter>().GetParent<IDXGIFactory>();
                swapchainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    BufferDescription = new ModeDescription(width, height, Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputWindow = hwnd,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Vortice.DXGI.Usage.RenderTargetOutput
                };
                _swapChain = dxgiFactory.CreateSwapChain(_device, swapchainDesc);
                dxgiFactory.MakeWindowAssociation(hwnd, WindowAssociationFlags.IgnoreAll);
                using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
                {
                    _rtv = _device.CreateRenderTargetView(backBuffer);
                }
            }
            else if (swapchainDesc.BufferDescription.Width != width || swapchainDesc.BufferDescription.Height != height)
            {
                if (_rtv != null)
                {
                    _rtv.Dispose();
                    _rtv = null;
                }
                _swapChain.ResizeBuffers(1, width, height, swapchainDesc.BufferDescription.Format, swapchainDesc.Flags);
                using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
                {
                    _rtv = _device.CreateRenderTargetView(backBuffer);
                }
            }

            _deviceContext.ClearRenderTargetView(_rtv, clearColor);
            _deviceContext.OMSetRenderTargets(_rtv);
            _deviceContext.RSSetViewport(0, 0, width, height);
        }

        public void EndFrame()
        {
            if (_swapChain != null)
            {
                _swapChain.Present(0, PresentFlags.None);
            }
        }

        public void Dispose()
        {
            if (_rtv != null)
            {
                _rtv.Dispose();
                _rtv = null;
            }
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
