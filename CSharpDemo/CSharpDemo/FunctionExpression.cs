using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp.Interop;

namespace CSharpDemo
{
    public class FunctionExpression
    {
        private readonly LLVMValueRef m_Function;

        internal FunctionExpression(ModuleExpression module, LLVMValueRef function)
        {
            this.Module = module;
            m_Function = function;
        }

        public ModuleExpression Module{get;}

        public LLVMValueRef Raw
        {
            get{return m_Function;}
        }

        public IReadOnlyList<LLVMValueRef> Parameters
        {
            get{return m_Function.Params;}
        }

        public BlockExpression MakeBlock(string name = "")
        {
            var block = this.Module.Raw.Context.AppendBasicBlock(m_Function, name);
            return new(this.Module, this, block);
        }

        public override string ToString()
        {
            return m_Function.ToString();
        }
    }
}
