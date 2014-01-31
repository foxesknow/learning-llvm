#include "learning.h"

using namespace llvm;

Function *Learning::CreateAdd(llvm::Module &module, llvm::LLVMContext &context)
{
	Function *function=cast<Function>(module.getOrInsertFunction("add",Type::getInt32Ty(context),Type::getInt32Ty(context),Type::getInt32Ty(context),nullptr));

	BasicBlock *block=BasicBlock::Create(context,"entry",function);

	auto argIt=function->arg_begin();
	Argument *argX=argIt++;
	Argument *argY=argIt++;

	argX->setName("x");
	argY->setName("y");

	//IRBuilder<> builder(context);
	Value *add=BinaryOperator::CreateAdd(argX,argY,"total",block);
	ReturnInst::Create(context,add,block);

	return function;
}

void Learning::TestAdd()
{
	LLVMContext context;

	// Create some module to put our function into it.
	OwningPtr<Module> module(new Module("test", context));

	// We are about to create the "fib" function:
	Function *addF = CreateAdd(*module, context);

	// Now we going to create JIT
	ExecutionEngine *engine = createEngine(*module);
	if (!engine) return;

	if(!verify(*module)) return;

	addF->print(outs());

	// Call the Fibonacci function with argument n:
	std::vector<GenericValue> args(2);
	args[0].IntVal = APInt(32, 1);
	args[1].IntVal = APInt(32, 2);
	GenericValue result = engine->runFunction(addF, args);

	// import result of execution
	outs() << "Result: " << result.IntVal << "\n";
}
