using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DearImGui.Silk.NET.Vulkan;

public class SilkWindow : IWindow
{
    public IWindow InternalWindow;

    public struct Options
    {
        public Vector2D<int> Size;
        public string Title;
    }
    public SilkWindow(Options o)
    {
        var options = WindowOptions.DefaultVulkan with
        {
            Size = o.Size,
            Title = o.Title
        };

        InternalWindow = Window.Create(options);
        InternalWindow.Initialize();

        if (InternalWindow.VkSurface is null)
        {
            throw new Exception("Windowing platform doesn't support Vulkan.");
        }
    }

    #region InternalWindowProxy

    public IWindowHost? Parent => InternalWindow.Parent;

    public IMonitor? Monitor { get => InternalWindow.Monitor; set => InternalWindow.Monitor = value; }
    public bool IsClosing { get => InternalWindow.IsClosing; set => InternalWindow.IsClosing = value; }

    public Rectangle<int> BorderSize => InternalWindow.BorderSize;

    public bool IsVisible { get => InternalWindow.IsVisible; set => InternalWindow.IsVisible = value; }
    public Vector2D<int> Position { get => InternalWindow.Position; set => InternalWindow.Position = value; }
    public Vector2D<int> Size { get => InternalWindow.Size; set => InternalWindow.Size = value; }
    public string Title { get => InternalWindow.Title; set => InternalWindow.Title = value; }
    public WindowState WindowState { get => InternalWindow.WindowState; set => InternalWindow.WindowState = value; }
    public WindowBorder WindowBorder { get => InternalWindow.WindowBorder; set => InternalWindow.WindowBorder = value; }

    public bool TransparentFramebuffer => InternalWindow.TransparentFramebuffer;

    public bool TopMost { get => InternalWindow.TopMost; set => InternalWindow.TopMost = value; }

    public IGLContext? SharedContext => InternalWindow.SharedContext;

    public string? WindowClass => InternalWindow.WindowClass;

    public nint Handle => InternalWindow.Handle;

    bool IView.IsClosing => ((IView)InternalWindow).IsClosing;

    public double Time => InternalWindow.Time;

    public Vector2D<int> FramebufferSize => InternalWindow.FramebufferSize;

    public bool IsInitialized => InternalWindow.IsInitialized;

    public bool ShouldSwapAutomatically { get => InternalWindow.ShouldSwapAutomatically; set => InternalWindow.ShouldSwapAutomatically = value; }
    public bool IsEventDriven { get => InternalWindow.IsEventDriven; set => InternalWindow.IsEventDriven = value; }
    public bool IsContextControlDisabled { get => InternalWindow.IsContextControlDisabled; set => InternalWindow.IsContextControlDisabled = value; }

    Vector2D<int> IViewProperties.Size => ((IViewProperties)InternalWindow).Size;

    public double FramesPerSecond { get => InternalWindow.FramesPerSecond; set => InternalWindow.FramesPerSecond = value; }
    public double UpdatesPerSecond { get => InternalWindow.UpdatesPerSecond; set => InternalWindow.UpdatesPerSecond = value; }

    public GraphicsAPI API => InternalWindow.API;

    public bool VSync { get => InternalWindow.VSync; set => InternalWindow.VSync = value; }

    public VideoMode VideoMode => InternalWindow.VideoMode;

    public int? PreferredDepthBufferBits => InternalWindow.PreferredDepthBufferBits;

    public int? PreferredStencilBufferBits => InternalWindow.PreferredStencilBufferBits;

    public Vector4D<int>? PreferredBitDepth => InternalWindow.PreferredBitDepth;

    public int? Samples => InternalWindow.Samples;

    public IGLContext? GLContext => InternalWindow.GLContext;

    public IVkSurface? VkSurface => InternalWindow.VkSurface;

    public INativeWindow? Native => InternalWindow.Native;

    public event Action<Vector2D<int>>? Move
    {
        add
        {
            InternalWindow.Move += value;
        }

        remove
        {
            InternalWindow.Move -= value;
        }
    }

    public event Action<WindowState>? StateChanged
    {
        add
        {
            InternalWindow.StateChanged += value;
        }

        remove
        {
            InternalWindow.StateChanged -= value;
        }
    }

    public event Action<string[]>? FileDrop
    {
        add
        {
            InternalWindow.FileDrop += value;
        }

        remove
        {
            InternalWindow.FileDrop -= value;
        }
    }

    public event Action<Vector2D<int>>? Resize
    {
        add
        {
            InternalWindow.Resize += value;
        }

        remove
        {
            InternalWindow.Resize -= value;
        }
    }

    public event Action<Vector2D<int>>? FramebufferResize
    {
        add
        {
            InternalWindow.FramebufferResize += value;
        }

        remove
        {
            InternalWindow.FramebufferResize -= value;
        }
    }

    public event Action? Closing
    {
        add
        {
            InternalWindow.Closing += value;
        }

        remove
        {
            InternalWindow.Closing -= value;
        }
    }

    public event Action<bool>? FocusChanged
    {
        add
        {
            InternalWindow.FocusChanged += value;
        }

        remove
        {
            InternalWindow.FocusChanged -= value;
        }
    }

    public event Action? Load
    {
        add
        {
            InternalWindow.Load += value;
        }

        remove
        {
            InternalWindow.Load -= value;
        }
    }

    public event Action<double>? Update
    {
        add
        {
            InternalWindow.Update += value;
        }

        remove
        {
            InternalWindow.Update -= value;
        }
    }

    public event Action<double>? Render
    {
        add
        {
            InternalWindow.Render += value;
        }

        remove
        {
            InternalWindow.Render -= value;
        }
    }

    public void SetWindowIcon(ReadOnlySpan<RawImage> icons)
    {
        InternalWindow.SetWindowIcon(icons);
    }

    public IWindow CreateWindow(WindowOptions opts)
    {
        return InternalWindow.CreateWindow(opts);
    }

    public void Initialize()
    {
        InternalWindow.Initialize();
    }

    public void DoRender()
    {
        InternalWindow.DoRender();
    }

    public void DoUpdate()
    {
        InternalWindow.DoUpdate();
    }

    public void DoEvents()
    {
        InternalWindow.DoEvents();
    }

    public void ContinueEvents()
    {
        InternalWindow.ContinueEvents();
    }

    public void Reset()
    {
        InternalWindow.Reset();
    }

    public void Close()
    {
        InternalWindow.Close();
    }

    public Vector2D<int> PointToClient(Vector2D<int> point)
    {
        return InternalWindow.PointToClient(point);
    }

    public Vector2D<int> PointToScreen(Vector2D<int> point)
    {
        return InternalWindow.PointToScreen(point);
    }

    public Vector2D<int> PointToFramebuffer(Vector2D<int> point)
    {
        return InternalWindow.PointToFramebuffer(point);
    }

    public object Invoke(Delegate d, params object[] args)
    {
        return InternalWindow.Invoke(d, args);
    }

    public void Run(Action onFrame)
    {
        InternalWindow.Run(onFrame);
    }

    public void Dispose()
    {
        InternalWindow.Dispose();
    }

    #endregion
}
