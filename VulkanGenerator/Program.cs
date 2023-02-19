using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using VulkanGenerator.Extensions;
using Platform = Microsoft.CodeAnalysis.Platform;

class VulkanCppLibrary : ILibrary
{
    Platform platform = Platform.X64;
    public void Setup(Driver driver)
    {
        var o = driver.Options;
        o.OutputDir = "VulkanBindings";
        o.GenerateFunctionTemplates = true;
        o.GeneratorKind = GeneratorKind.CSharp;
        o.GenerateDebugOutput = true;
        o.GenerateClassTemplates = true;
        o.Verbose = true;
        o.Quiet = false;
        o.GenerateObjectOverrides = true;
        o.GenerateInternalImports = true;
        o.OutputInteropIncludes = true;
        o.UseSpan = true;
        o.MarshalCharAsManagedChar = true;
        o.GenerationOutputMode = GenerationOutputMode.FilePerUnit;
        o.GenerateDefaultValuesForArguments = true;
        o.OutputDir = @"C:\repo\DearImGui\VulkanCppSharp";

        //driver.ParserOptions.NoBuiltinIncludes = true;
        driver.ParserOptions.NoStandardIncludes = true;

        driver.ParserOptions.SetupMSVC(VisualStudioVersion.Latest);

        var m = o.AddModule("vk");
        m.IncludeDirs.Add(@"C:\repo\DearImGui\vk\Vulkan-Headers\include");
        m.OutputNamespace = "vkcpp";
        
        m.Headers.Add("vulkan/vulkan_core.h");
        m.Headers.Add("vulkan/vulkan.hpp");
        //m.LibraryDirs.Add("@\"C:\\Windows\\System32");
        //m.Libraries.Add(@"vulkan.dll");
        m.Defines.Add("VULKAN_HPP_NO_EXCEPTIONS");
        m.Defines.Add("VULKAN_HPP_DISABLE_ENHANCED_MODE");
        m.Defines.Add("_ALLOW_COMPILER_AND_STL_VERSION_MISMATCH");
        


        driver.ParserOptions.TargetTriple = platform switch
        {
            Platform.X86 => "i686-pc-win32-msvc",
            Platform.X64 => "x86_64-pc-win32-msvc",
            _ => throw new NotSupportedException(platform.ToString())
        };

        
        //parserOptions.AddIncludeDirs("C:/repo/DearImGui/vk/Vulkan-Headers/include");

        

    }

    public void SetupPasses(Driver driver)
    {
        
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        
    }

    static class Program
    {
        public static void Main(string[] args)
        {
            var outwriter = File.CreateText("gen.log");
            var consoleSnatcher = new AggregateConsoleOut(outwriter);
            ConsoleDriver.Run(new VulkanCppLibrary());
            consoleSnatcher.Dispose();
        }
    }
}