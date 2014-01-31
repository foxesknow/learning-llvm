#include "learning.h"

using namespace llvm;

Function *Learning::CreateMax(llvm::Module &module, llvm::LLVMContext &context)
{
	Function *function=cast<Function>(module.getOrInsertFunction("add",Type::getInt32Ty(context),Type::getInt32Ty(context),Type::getInt32Ty(context),nullptr));

	BasicBlock *block=BasicBlock::Create(context,"entry",function);
	BasicBlock *ifTrue=BasicBlock::Create(context,"ifTrue",function);
	BasicBlock *ifFalse=BasicBlock::Create(context,"ifFalse",function);
	BasicBlock *ret=BasicBlock::Create(context,"retTrue",function);

	auto argIt=function->arg_begin();
	Argument *argX=argIt++;
	argX->setName("x");

	Argument *argY=argIt++;
	argY->setName("y");

	AllocaInst *value=new AllocaInst(IntegerType::get(context,32),"value",block);

	ICmpInst *condition=new ICmpInst(*block,CmpInst::ICMP_SGT,argX,argY,"compare");
	BranchInst::Create(ifTrue,ifFalse,condition,block);

	new StoreInst(argX,value,ifTrue);
	BranchInst::Create(ret,ifTrue);

	new StoreInst(argY,value,ifFalse);
	BranchInst::Create(ret,ifFalse);

	LoadInst *load=new LoadInst(value,"",ret);
	ReturnInst::Create(context,load,ret);

	return function;
}

void Learning::TestMax()
{
	LLVMContext context;

	// Create some module to put our function into it.
	OwningPtr<Module> module(new Module("test", context));

	Function *max=CreateMax(*module, context);
	max->print(outs());

	// Now we going to create JIT
	ExecutionEngine *engine=createEngine(*module);
	if (!engine) return;

	if(!verify(*module)) return;

	// Call the Fibonacci function with argument n:
	std::vector<GenericValue> args(2);
	args[0].IntVal=APInt(32, 7);
	args[1].IntVal=APInt(32, 5);
	GenericValue result=engine->runFunction(max,args);

	// import result of execution
	outs() << "Result: " << result.IntVal << "\n";
}


