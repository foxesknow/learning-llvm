using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp.Interop;

namespace CSharpDemo
{
    public class ModuleExpression : IDisposable
    {
        private LLVMModuleRef m_Module;

        public ModuleExpression(string name)
        {
            m_Module = LLVMModuleRef.CreateWithName(name);
        }

        public LLVMModuleRef Raw
        {
            get{return m_Module;}
        }

        public FunctionExpression AddFunction(string name, LLVMTypeRef returnType, params LLVMTypeRef[] parameters)
        {
            var functionProtoType = LLVMTypeRef.CreateFunction(returnType, parameters);
            var function = m_Module.AddFunction(name, functionProtoType);

            return new(this, function);
        }

        public void Dispose()
        {
            m_Module.Dispose();
        }
    }
}
