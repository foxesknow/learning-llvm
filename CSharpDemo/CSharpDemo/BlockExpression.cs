﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp.Interop;

namespace CSharpDemo
{
    public class BlockExpression
    {
        private readonly LLVMBasicBlockRef m_Block;
        
        internal BlockExpression(ModuleExpression module, FunctionExpression function, LLVMBasicBlockRef block)
        {
            this.Module = module;
            this.Function = function;
            m_Block = block;
        }

        public ModuleExpression Module{get;}
        
        public FunctionExpression Function{get;}

        public LLVMBasicBlockRef Raw
        {
            get{return m_Block;}
        }

        public LLVMBuilderRef MakeBuilder()
        {
            var builder = this.Module.Raw.Context.CreateBuilder();
            builder.PositionAtEnd(m_Block);

            return builder;
        }

        public BlockExpression IfThenElse(LLVMValueRef predicate, ExitBodyBuilder ifTrue, ExitBodyBuilder ifFalse)
        {
            var ifTrueBlock = this.Function.MakeBlock("ifTrue");
            var ifFalseBlock = this.Function.MakeBlock("ifFalse");
            var continuation = this.Function.MakeBlock("continuation");

            ifTrue(ifTrueBlock, continuation);
            ifFalse(ifFalseBlock, continuation);

            using(var builder = this.MakeBuilder())
            {
                builder.BuildCondBr(predicate, ifTrueBlock.Raw, ifFalseBlock.Raw);
            }

            return continuation;
        }

        public BlockExpression While(Func<LLVMBuilderRef, LLVMValueRef> predicate, LoopBodyBuilder loopBuilder)
        {
            var whileLoop = this.Function.MakeBlock("whileLoop");
            var whileBody = this.Function.MakeBlock("whileBody");
            var breakBlock = this.Function.MakeBlock("breakBlock");

            loopBuilder(whileBody, breakBlock, whileLoop);

            using(var builder = MakeBuilder())
            {
                builder.BuildBr(whileLoop.Raw);

                using(var whileLoopBuilder = whileLoop.MakeBuilder())
                {
                    whileLoopBuilder.BuildCondBr(predicate(whileLoopBuilder), whileBody.Raw, breakBlock.Raw);
                    loopBuilder(whileBody, breakBlock, whileLoop);

                    using(var retestBuilder = whileBody.MakeBuilder())
                    {
                        retestBuilder.BuildBr(whileLoop.Raw);
                    }
                }
            }

            return breakBlock;
        }
    }
}
