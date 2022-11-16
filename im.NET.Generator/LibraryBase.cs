﻿using System.Runtime.CompilerServices;
using CppSharp;
using CppSharp.AST;
using CppSharp.Passes;
using im.NET.Generator.Logging;

namespace im.NET.Generator;

public abstract class LibraryBase : ILibrary
{
    #region ILibrary Members

    public abstract void Setup(Driver driver);

    public abstract void SetupPasses(Driver driver);

    public abstract void Preprocess(Driver driver, ASTContext ctx);

    public abstract void Postprocess(Driver driver, ASTContext ctx);

    #endregion

    protected static TranslationUnit GetImGuiTranslationUnit(ASTContext ctx)
    {
        return ctx.TranslationUnits.Single(s => s.FileName == "imgui.h");
    }

    protected static void Ignore(ASTContext ctx, string? className, string? memberName, IgnoreType ignoreType)
    {
        switch (ignoreType)
        {
            case IgnoreType.Class:
                ctx.IgnoreClassWithName(className);
                return;
            case IgnoreType.Function:
                ctx.IgnoreFunctionWithName(memberName);
                return;
            case IgnoreType.Method:
                ctx.IgnoreClassMethodWithName(className, memberName);
                return;
            case IgnoreType.Property:
                ctx.FindCompleteClass(className).Properties.Single(s => s.Name == memberName).ExplicitlyIgnore();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(ignoreType), ignoreType, null);
        }
    }

    protected static void RemovePass<T>(Driver driver, [CallerMemberName] string memberName = null!) where T : TranslationUnitPass
    {
        var count = driver.Context.TranslationUnitPasses.Passes.RemoveAll(s => s is T);

        using (new ConsoleColorScope(null, ConsoleColor.Yellow))
            Console.WriteLine($"Removed {count} passes of type {typeof(T)} in {memberName}");
    }
}