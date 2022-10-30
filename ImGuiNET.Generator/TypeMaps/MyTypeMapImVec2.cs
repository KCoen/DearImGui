﻿using System.Numerics;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

namespace ImGuiNET.Generator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImVec2", GeneratorKind.CSharp)]
internal sealed class MyTypeMapImVec2 : TypeMap
{
    public override bool DoesMarshalling => base.DoesMarshalling;

    public override bool IsIgnored => base.IsIgnored;

    public override bool IsValueType => true;

    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CILType(typeof(Vector2));
    }

    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
                ctx.Return.Write("/* CSharpMarshalToNative func null, return var name null */");
            }
            else
            {
                ctx.Return.Write(ctx.Parameter.Name);
                ctx.Return.Write("/* CSharpMarshalToNative func null, return var name NOT null */");
            }
        }
        else
        {
            if (ctx.ReturnVarName == null)
            {
                if (ctx.Function.Name == "AddRectFilled" && ctx.Parameter.Name == "p_min")
                {
                    // TODO delete
                }

                if (ctx.Function.Name == "PlotLines" && ctx.Parameter.Name == "graph_size")
                {
                    // TODO delete
                }

                if (ctx.Function.Name == "CalcCustomRectUV" && ctx.Parameter.Name == "out_uv_min")
                {
                    // TODO delete
                }

                if (ctx.Parameter.IsConst || ctx.Parameter.HasDefaultValue is false)
                {
                    ctx.Return.Write($"new IntPtr(Unsafe.AsPointer(ref {ctx.Parameter.Name}))"); // 8 errors with this
                }
                else
                {
                    ctx.Return.Write($"{ctx.Parameter.Name}"); // 79 errors with this
                }

                ctx.Return.Write("/* CSharpMarshalToNative func NOT null, return var name null */");
            }
            else
            {
                ctx.Return.Write("/* CSharpMarshalToNative func NOT null, return var name NOT null */");
            }
        }
    }

    public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        var ctxReturnVarName = ctx.ReturnVarName;
        if (ctxReturnVarName == null)
        {
            throw new InvalidOperationException();
        }

        if (ctx.Function == null)
        {
            if (ctxReturnVarName == null)
            {
                ctx.Return.Write("/* CSharpMarshalToManaged func null, return var name null */");
            }
            else
            {
                ctx.Return.Write(ctxReturnVarName);
                ctx.Return.Write("/* CSharpMarshalToManaged func null, return var name NOT null */");
            }
        }
        else
        {
            if (ctxReturnVarName == null)
            {
                ctx.Return.Write("/* CSharpMarshalToManaged func NOT null, return var name null */");
            }
            else
            {
                ctx.Return.Write($"Unsafe.AsRef<global::System.Numerics.Vector2>(&{ctxReturnVarName})");
                ctx.Return.Write("/* CSharpMarshalToManaged func NOT null, return var name NOT null */");
            }
        }
    }
}