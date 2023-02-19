using Silk.NET.Core.Native;
using Silk.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan;
using System.Runtime.CompilerServices;
using Silk.NET.SDL;

namespace DearImGui.Silk.NET.Vulkan.Vulkan;

unsafe internal class VulkanInstance
{
    public class VulkanConfig
    {
        public bool Debug = true;

        public string Title = "Blackware";
        public string Engine = "Silk.NET.Vulkan";

        public uint VulkanMajor = 1;
        public uint VulkanMinor = 3;
        public uint VulkanPatch = 0;

        public string[] validationLayers = new string[] { };

        public string[] deviceExtensions = new[]
        {
            KhrSwapchain.ExtensionName
        };
    }

    private List<string> DeviceExtensions = new List<string>();
    private List<string> ValidationLayers = new List<string>();
    private List<string> InstanceExtensions = new List<string>();

    private Vk vk;
    private KhrSurface KhrSurfaceExt;
    private SilkWindow window;
    private Queue Queue;

    private Instance instance;

    private void DebugMessageSettings(ref DebugUtilsMessengerCreateInfoEXT s)
    {
        s.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
        
        s.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.DebugUtilsMessageSeverityVerboseBitExt |
                            DebugUtilsMessageSeverityFlagsEXT.DebugUtilsMessageSeverityWarningBitExt |
                            DebugUtilsMessageSeverityFlagsEXT.DebugUtilsMessageSeverityErrorBitExt;
        
        s.MessageType =     DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypeGeneralBitExt |
                            DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypePerformanceBitExt |
                            DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypeValidationBitExt;
        
        s.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT) DebugCallback;
    }

    static private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        Console.WriteLine($"validation layer:" + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

        return Vk.False;
    }


    public VulkanInstance(VulkanConfig config, SilkWindow _window)
    {
        window = _window;
        
        if (config.Debug)
        {
            InstanceExtensions.Append(ExtDebugUtils.ExtensionName);
            ValidationLayers.Append("VK_LAYER_KHRONOS_validation");
        }

        InstanceExtensions.AddRange(GetGlfwExtensions());

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(config.Title),
            ApplicationVersion = new Version32(1, 0, 0),
            PEngineName = (byte*)Marshal.StringToHGlobalAnsi(config.Engine),
            EngineVersion = new Version32(1, 0, 0),
            ApiVersion = new Version32(config.VulkanMajor, config.VulkanMinor, config.VulkanPatch)
        };

        InstanceCreateInfo createInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            EnabledLayerCount = (uint)ValidationLayers.Count(),
            PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(ValidationLayers.ToArray()),
            EnabledExtensionCount = (uint)InstanceExtensions.Count(),
            PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(InstanceExtensions.ToArray())
        };

        if (config.Debug)
        {
            DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new();
            DebugMessageSettings(ref debugCreateInfo);
            createInfo.PNext = &debugCreateInfo;
        }
        vk = Vk.GetApi(ref createInfo, out instance);
        vk.CurrentInstance = instance;

        // Get Extension Api's
        if (!vk!.TryGetInstanceExtension<KhrSurface>(instance, out KhrSurfaceExt))
        {
            throw new NotSupportedException("KHR_surface extension not found.");
        }

        // Create Surface
        var surface = window!.VkSurface!.Create<AllocationCallbacks>(instance.ToHandle(), null).ToSurface();

        var (physicalDevice, queufam) = FindPhyiscalDeviceAndFamilyForSurface(surface);
        var device = CreateDevice(physicalDevice, queufam);
        vk!.GetDeviceQueue(device, queufam, 0, out Queue);


        /*
        CreateSwapChain();
        CreateImageViews(); // Simplified
        CreateRenderPass(); // Optional
        CreateGraphicsPipeline(); // Push Constants
        CreateFramebuffers();
        CreateCommandPool();
        CreateCommandBuffers();
        CreateSyncObjects();
        */
    }

    private string[] GetGlfwExtensions()
    {
        var glfwExtensions = window.VkSurface!.GetRequiredExtensions(out var glfwExtensionCount);
        var extensions = SilkMarshal.PtrToStringArray((nint)glfwExtensions, (int)glfwExtensionCount);
        return extensions;
    }

    private (PhysicalDevice, uint) FindPhyiscalDeviceAndFamilyForSurface(SurfaceKHR surface)
    {
        uint devicedCount = 0;
        vk!.EnumeratePhysicalDevices(instance, ref devicedCount, null);

        if (devicedCount == 0)
        {
            throw new Exception("failed to find GPUs with Vulkan support!");
        }

        var physicalDevices = new PhysicalDevice[devicedCount];
        fixed (PhysicalDevice* devicesPtr = physicalDevices)
        {
            vk!.EnumeratePhysicalDevices(instance, ref devicedCount, devicesPtr);
        }

        foreach (var physicaldevice in physicalDevices)
        {
            if (IsDeviceSuitable(physicaldevice))
            {
                return (physicaldevice, FindQueueFamForSurface(physicaldevice, surface));
            }
        }

        throw new Exception("failed to find a suitable GPU!");

        bool IsDeviceSuitable(PhysicalDevice device)
        {
            if (device.Handle == 0)
            {
                return false;
            }

            var fam = FindQueueFamForSurface(device, surface);

            bool extensionsSupported = CheckDeviceExtensionsSupport(device);

            bool swapChainAdequate = false;
            if (extensionsSupported)
            {
                var swapChainSupport = QuerySwapChainSupport(device, surface);
                swapChainAdequate = swapChainSupport.Formats.Any() && swapChainSupport.PresentModes.Any();
            }

            return extensionsSupported && swapChainAdequate;

            bool CheckDeviceExtensionsSupport(PhysicalDevice device)
            {
                uint extentionsCount = 0;
                vk!.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, null);

                var availableExtensions = new ExtensionProperties[extentionsCount];
                fixed (ExtensionProperties* availableExtensionsPtr = availableExtensions)
                {
                    vk!.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extentionsCount, availableExtensionsPtr);
                }

                var availableExtensionNames = availableExtensions.Select(extension => Marshal.PtrToStringAnsi((IntPtr)extension.ExtensionName)).ToHashSet();

                return DeviceExtensions.All(availableExtensionNames.Contains);
            }

            SwapChainSupportDetails QuerySwapChainSupport(PhysicalDevice physicalDevice, SurfaceKHR surface)
            {
                var details = new SwapChainSupportDetails();

                KhrSurfaceExt.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, out details.Capabilities);

                uint formatCount = 0;
                KhrSurfaceExt.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, ref formatCount, null);

                if (formatCount != 0)
                {
                    details.Formats = new SurfaceFormatKHR[formatCount];
                    fixed (SurfaceFormatKHR* formatsPtr = details.Formats)
                    {
                        KhrSurfaceExt.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, ref formatCount, formatsPtr);
                    }
                }
                else
                {
                    details.Formats = Array.Empty<SurfaceFormatKHR>();
                }

                uint presentModeCount = 0;
                KhrSurfaceExt.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, ref presentModeCount, null);

                if (presentModeCount != 0)
                {
                    details.PresentModes = new PresentModeKHR[presentModeCount];
                    fixed (PresentModeKHR* formatsPtr = details.PresentModes)
                    {
                        KhrSurfaceExt.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, ref presentModeCount, formatsPtr);
                    }

                }
                else
                {
                    details.PresentModes = Array.Empty<PresentModeKHR>();
                }

                return details;
            }
        }
        uint FindQueueFamForSurface(PhysicalDevice device, SurfaceKHR surface)
        {
            uint queueFamilityCount = 0;
            vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, null);

            var queueFamilies = new QueueFamilyProperties[queueFamilityCount];
            fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies)
            {
                vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, queueFamiliesPtr);
            }


            uint i = 0;
            foreach (var queueFamily in queueFamilies)
            {
                KhrSurfaceExt!.GetPhysicalDeviceSurfaceSupport(device, i, surface, out var presentSupport);

                if (presentSupport)
                {
                    return i;
                }
                i++;
            }

            throw new Exception("Could not find Queue Family");
        }
    }

    
    private Device CreateDevice(PhysicalDevice physicalDevice, uint queueFamily)
    {
        var uniqueQueueFamilies = new[] { queueFamily };
        uniqueQueueFamilies = uniqueQueueFamilies.ToArray();

        using var mem = GlobalMemory.Allocate(uniqueQueueFamilies.Length * sizeof(DeviceQueueCreateInfo));
        var queueCreateInfos = (DeviceQueueCreateInfo*)Unsafe.AsPointer(ref mem.GetPinnableReference());

        float queuePriority = 1.0f;
        for (int i = 0; i < uniqueQueueFamilies.Length; i++)
        {
            queueCreateInfos[i] = new()
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = uniqueQueueFamilies[i],
                QueueCount = 1
            };

            queueCreateInfos[i].PQueuePriorities = &queuePriority;
        }

        PhysicalDeviceFeatures deviceFeatures = new();

        DeviceCreateInfo createInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            QueueCreateInfoCount = (uint)uniqueQueueFamilies.Length,
            PQueueCreateInfos = queueCreateInfos,

            PEnabledFeatures = &deviceFeatures,

            EnabledExtensionCount = (uint)DeviceExtensions.Count(),
            PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(DeviceExtensions.ToArray())
        };

        if (ValidationLayers.Count() > 0)
        {
            createInfo.EnabledLayerCount = (uint)ValidationLayers.Count();
            createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(ValidationLayers.ToArray());
        }
        else
        {
            createInfo.EnabledLayerCount = 0;
        }

        if (vk!.CreateDevice(physicalDevice, in createInfo, null, out var device) != Result.Success)
        {
            throw new Exception("failed to create logical device!");
        }
        return device;
    }
}


