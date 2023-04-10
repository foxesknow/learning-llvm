using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDemo
{
    public delegate Expression LoopBodyBuilder(BlockExpression @break, BlockExpression @continue);  
}
