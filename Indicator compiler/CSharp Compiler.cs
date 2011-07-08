// CSharp_Compiler Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// CSharp_Compiler manages the compilation of source code to an assembly.
    /// This class is thread safe, so multiple threads are capable 
    /// to utilize it simultaneously.
    /// </summary>
    public class CSharp_Compiler
    {
        // Provides the actual compilation of source code.
        volatile CSharpCodeProvider codeProvider;

        // Represents the parameters used to invoke a compiler.
        CompilerParameters compilationParameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CSharp_Compiler()
        {
            codeProvider = new CSharpCodeProvider();
            compilationParameters = new CompilerParameters();

            // Make sure we conduct all the operations "in memory".
            compilationParameters.GenerateInMemory = true;

            return;
        }

        /// <summary>
        /// For the source code to compile, it needs to have a reference to assemblies
        /// to use the IL code inside them.
        /// </summary>
        /// <param name="assembly">An assembly to add.</param>
        public void AddReferencedAssembly(Assembly assembly)
        {
            lock (this)
            {
                compilationParameters.ReferencedAssemblies.Add(assembly.Location);
            }

            return;
        }

        /// <summary>
        /// Compile a single source file to assembly.
        /// </summary>
        /// <param name="compilerErrors">Compiler errors, if any.</param>
        /// <returns>Compiled assembly or null.</returns>
        public Assembly CompileSource(string source, out Dictionary<string, int> compilerErrors)
        {
            compilerErrors = new Dictionary<string, int>();

            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilationParameters, source);

            if (compilerResults.Errors.Count > 0)
            {   // Compilation failed.
                foreach (CompilerError error in compilerResults.Errors)
                {
                    string sErrorMessage = "Line " + error.Line + " Column " + error.Column + ": " + error.ErrorText + ".";
                    int    iErrorLine    = error.Line;

                    if (!compilerErrors.ContainsKey(sErrorMessage))
                        compilerErrors.Add(sErrorMessage, iErrorLine);
                }

                return null;
            }

            return compilerResults.CompiledAssembly;
        }
    }
}
