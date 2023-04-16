using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp.Interop;

namespace CSharpDemo
{
    public sealed class Builder : IDisposable
    {
        private LLVMBuilderRef m_Builder;

        public Builder(LLVMBuilderRef builder)
        {
            m_Builder = builder;
        }

        void IDisposable.Dispose()
        {
            m_Builder.Dispose();
            m_Builder = default;
        }

        public LLVMValueRef AllocateStackVariable(LLVMTypeRef type, string? name)
        {
            return m_Builder.BuildAlloca(type, name ?? "");
        }

        public LLVMValueRef Load(LLVMTypeRef type, LLVMValueRef value)
        {
            return m_Builder.BuildLoad2(type, value);
        }

        public LLVMValueRef Store(LLVMValueRef target, LLVMValueRef value)
        {
            return m_Builder.BuildStore(value, target);
        }

        public LLVMValueRef Add(LLVMValueRef lhs, LLVMValueRef rhs)
        {
            return m_Builder.BuildAdd(lhs, rhs);
        }

        public LLVMValueRef Return(LLVMValueRef value)
        {
            return m_Builder.BuildRet(value);
        }

        public LLVMValueRef Branch(LLVMBasicBlockRef destination)
        {
            return m_Builder.BuildBr(destination);
        }

        public LLVMValueRef IfThenElse(LLVMValueRef @if, LLVMBasicBlockRef then, LLVMBasicBlockRef @else)
        {
            return m_Builder.BuildCondBr(@if, then, @else);
        }

        public LLVMValueRef Compare(LLVMIntPredicate predicate, LLVMValueRef lhs, LLVMValueRef rhs, string? name)
        {
            return m_Builder.BuildICmp(predicate, lhs, rhs , name ?? "");
        }
    }
}
