#include <string>

#include "learning.h"

Learning::Learning()
{
	llvm::InitializeNativeTarget();
}

bool Learning::verify(llvm::Module &module)
{
	errs() << "verifying... ";
	if(llvm::verifyModule(module))
	{
		errs() << " error constructing module\n";
		return false;
	}
	else
	{
		errs() << "passed\n";
	}

	return true;
}

llvm::ExecutionEngine *Learning::createEngine(llvm::Module &module)
{
	std::string errStr;
	llvm::ExecutionEngine *engine=
		llvm::EngineBuilder(&module)
		.setErrorStr(&errStr)
		.setEngineKind(EngineKind::JIT)
		.create();

	if (!engine)
	{
		errs() << "Failed to construct ExecutionEngine: " << errStr << "\n";
	}

	return engine;
}
