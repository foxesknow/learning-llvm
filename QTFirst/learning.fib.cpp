#include "learning.h"

using namespace llvm;

Function *Learning::CreateFib(Module &module, LLVMContext &Context)
{
  // Create the fib function and insert it into module M.  This function is said
  // to return an int and take an int parameter.
  Function *FibF =
	cast<Function>(module.getOrInsertFunction("fib", Type::getInt32Ty(Context),
										  Type::getInt32Ty(Context),
										  (Type *)0));

  // Add a basic block to the function.
  BasicBlock *BB = BasicBlock::Create(Context, "EntryBlock", FibF);

  // Get pointers to the constants.
  Value *One = ConstantInt::get(Type::getInt32Ty(Context), 1);
  Value *Two = ConstantInt::get(Type::getInt32Ty(Context), 2);

  // Get pointer to the integer argument of the add1 function...
  Argument *ArgX = FibF->arg_begin();   // Get the arg.
  ArgX->setName("AnArg");            // Give it a nice symbolic name for fun.

  // Create the true_block.
  BasicBlock *RetBB = BasicBlock::Create(Context, "return", FibF);
  // Create an exit block.
  BasicBlock* RecurseBB = BasicBlock::Create(Context, "recurse", FibF);

  // Create the "if (arg <= 2) goto exitbb"
  Value *CondInst = new ICmpInst(*BB, ICmpInst::ICMP_SLE, ArgX, Two, "cond");
  BranchInst::Create(RetBB, RecurseBB, CondInst, BB);

  // Create: ret int 1
  ReturnInst::Create(Context, One, RetBB);

  // create fib(x-1)
  Value *Sub = BinaryOperator::CreateSub(ArgX, One, "arg", RecurseBB);
  CallInst *CallFibX1 = CallInst::Create(FibF, Sub, "fibx1", RecurseBB);
  CallFibX1->setTailCall();

  // create fib(x-2)
  Sub = BinaryOperator::CreateSub(ArgX, Two, "arg", RecurseBB);
  CallInst *CallFibX2 = CallInst::Create(FibF, Sub, "fibx2", RecurseBB);
  CallFibX2->setTailCall();


  // fib(x-1)+fib(x-2)
  Value *Sum = BinaryOperator::CreateAdd(CallFibX1, CallFibX2, "addresult", RecurseBB);

  // Create the return instruction and add it to the basic block
  ReturnInst::Create(Context, Sum, RecurseBB);

  return FibF;
}

void Learning::TestFib()
{
  LLVMContext context;

  // Create some module to put our function into it.
  OwningPtr<Module> module(new Module("test", context));

  // We are about to create the "fib" function:
  Function *function = CreateFib(*module, context);

  // Now we going to create JIT
  ExecutionEngine *engine = createEngine(*module);
  if (!engine) return;

  if(!verify(*module)) return;


  // Call the Fibonacci function with argument n:
  std::vector<GenericValue> args(1);
  args[0].IntVal = APInt(32, 6);
  GenericValue result = engine->runFunction(function, args);

  // import result of execution
  outs() << "Result: " << result.IntVal << "\n";

}
