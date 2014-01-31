#ifndef LEARNING_H
#define LEARNING_H

#include "llvm/IR/LLVMContext.h"
#include "llvm/IR/Module.h"
#include "llvm/IR/DerivedTypes.h"
#include "llvm/IR/Constants.h"
#include "llvm/IR/Instructions.h"
#include "llvm/Analysis/Verifier.h"
#include "llvm/ExecutionEngine/JIT.h"
#include "llvm/ExecutionEngine/Interpreter.h"
#include "llvm/ExecutionEngine/GenericValue.h"
#include "llvm/Support/raw_ostream.h"
#include "llvm/Support/TargetSelect.h"
#include "llvm/IR/IRBuilder.h"

class Learning
{
protected:
	bool verify(llvm::Module &module);

	llvm::ExecutionEngine *createEngine(llvm::Module &module);

public:
	Learning();

	void TestFib();
	llvm::Function *CreateFib(llvm::Module &module, llvm::LLVMContext &Context);

	void TestAdd();
	llvm::Function *CreateAdd(llvm::Module &module, llvm::LLVMContext &Context);

	void TestAddIR();
	llvm::Function *CreateAddIR(llvm::Module &module, llvm::LLVMContext &Context);

	void TestMax();
	llvm::Function *CreateMax(llvm::Module &module, llvm::LLVMContext &context);
};

#endif // LEARNING_H
