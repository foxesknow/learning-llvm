using System;
using System.Runtime.InteropServices;

//using LLVMSharp;
using LLVMSharp.Interop;

namespace CSharpDemo
{
    internal class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int BinaryInt32Operation(int op1, int op2);

        static void Main(string[] args)
        {
            MakeMax();
            Console.WriteLine("Hello, World!");
        }

        static void MakeMax()
        {
            using var module = LLVMModuleRef.CreateWithName("Max");
            var functionProtoType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, new[]{LLVMTypeRef.Int32, LLVMTypeRef.Int32});
            var function = module.AddFunction("Max", functionProtoType);

            var block = module.Context.AppendBasicBlock(function, "entry");
            var ifTrue = module.Context.AppendBasicBlock(function, "ifTrue");
            var ifFalse = module.Context.AppendBasicBlock(function, "ifFalse");
            var ret = module.Context.AppendBasicBlock(function, "ret");

            var x = function.Params[0];
            x.Name = "x";

            var y = function.Params[1];
            y.Name = "y";

            var builder = module.Context.CreateBuilder();

            builder.PositionAtEnd(block);

            var value = builder.BuildAlloca(LLVMTypeRef.Int32, "value");
            var compare = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, x, y, "compare");
            builder.BuildCondBr(compare, ifTrue, ifFalse);

            // IfTrue
            builder.PositionAtEnd(ifTrue);
            builder.BuildStore(x, value);
            builder.BuildBr(ret);

            // IfFalse
            builder.PositionAtEnd(ifFalse);
            builder.BuildStore(y, value);
            builder.BuildBr(ret);

            // Return
            builder.PositionAtEnd(ret);
            var load = builder.BuildLoad2(LLVMTypeRef.Int32, value);
            builder.BuildRet(load);

            Console.WriteLine(function);

            _ = LLVM.InitializeNativeTarget();
            _ = LLVM.InitializeNativeAsmParser();
            _ = LLVM.InitializeNativeAsmPrinter();
            var engine = module.CreateMCJITCompiler();
            var max = engine.GetPointerToGlobal<BinaryInt32Operation>(function);
            var result = max(10, 12);

            Console.WriteLine(result);
        }
    }
}