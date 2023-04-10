using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDemo
{
    public delegate void LoopBodyBuilder(BlockExpression body, BlockExpression @break, BlockExpression @continue);  
}
