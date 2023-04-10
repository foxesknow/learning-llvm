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
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int UnaryInt32Operation(int op1);

        static void Main(string[] args)
        {
            Add_Expression();
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

        static void MakeMax_Expression()
        {
            using(var module = new ModuleExpression("Max"))
            {
                var function = module.AddFunction("Max", LLVMTypeRef.Int32, LLVMTypeRef.Int32, LLVMTypeRef.Int32);
                var x = function.Parameters[0];
                var y = function.Parameters[1];

                var entry = function.MakeBlock("entry");
                using(var entryBuilder = entry.MakeBuilder())
                {
                    var value = entryBuilder.BuildAlloca(LLVMTypeRef.Int32, "value");
                    var predicate = entryBuilder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, x, y, "compare");

                    var continuation = entry.IfThenElse
                    (
                        predicate,
                        ifTrue: (block, exit) =>
                        {
                            using var builder = block.MakeBuilder();
                            builder.BuildStore(x, value);
                            builder.BuildBr(exit.Raw);
                        },
                        ifFalse: (block, exit) =>
                        {
                            using var builder = block.MakeBuilder();
                            builder.BuildStore(y, value);
                            builder.BuildBr(exit.Raw);
                        }
                    );

                    using(var builder = continuation.MakeBuilder())
                    {
                        var load = builder.BuildLoad2(LLVMTypeRef.Int32, value);
                        builder.BuildRet(load);
                    }
                }

                Console.WriteLine(function);

                _ = LLVM.InitializeNativeTarget();
                _ = LLVM.InitializeNativeAsmParser();
                _ = LLVM.InitializeNativeAsmPrinter();
                var engine = module.Raw.CreateMCJITCompiler();
                var max = engine.GetPointerToGlobal<BinaryInt32Operation>(function.Raw);
                var result = max(10, 12);

                Console.WriteLine(result);
            }
        }

        static void Add_Expression()
        {
            using(var module = new ModuleExpression("Adder"))
            {
                var function = module.AddFunction("Adder", LLVMTypeRef.Int32, LLVMTypeRef.Int32);
                var repetitions = function.Parameters[0];

                var entry = function.MakeBlock("entry");
                using(var entryBuilder = entry.MakeBuilder())
                {
                    var zero = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0);
                    var one = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 1);
                    var two = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 2);

                    var value = entryBuilder.BuildAlloca(LLVMTypeRef.Int32, "value");
                    entryBuilder.BuildStore(value, zero);
                    
                    var counter = entryBuilder.BuildAlloca(LLVMTypeRef.Int32, "counter");
                    entryBuilder.BuildStore(counter, zero);
                    
                    var continuation = entry.While
                    (
                        builder => builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, counter, repetitions, "compare"),
                        (body, @break, @continue) =>
                        {
                            using var builder = body.MakeBuilder();
                            builder.BuildStore(value, builder.BuildAdd(value, two));
                            builder.BuildStore(counter, builder.BuildAdd(counter, one));
                        }
                    );

                    using(var builder = continuation.MakeBuilder())
                    {
                        var load = builder.BuildLoad2(LLVMTypeRef.Int32, value);
                        builder.BuildRet(load);
                    }
                }

                Console.WriteLine(function);

                _ = LLVM.InitializeNativeTarget();
                _ = LLVM.InitializeNativeAsmParser();
                _ = LLVM.InitializeNativeAsmPrinter();
                var engine = module.Raw.CreateMCJITCompiler();
                var adder = engine.GetPointerToGlobal<UnaryInt32Operation>(function.Raw);
                var result = adder(10);

                Console.WriteLine(result);
            }
        }
    }
}