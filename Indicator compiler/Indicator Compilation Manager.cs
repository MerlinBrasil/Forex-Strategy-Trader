// Indicator_Compilation_Manager class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Manages the operation of indicators.
    /// </summary>
    public class Indicator_Compilation_Manager
    {
        CSharp_Compiler compiler;

        List<Indicator> listCustomIndicators = new List<Indicator>();

        /// <summary>
        /// Gets a list of the loaded custom indicators
        /// </summary>
        public List<Indicator> CustomIndicatorsList { get { return listCustomIndicators; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Indicator_Compilation_Manager()
        {
            compiler = new CSharp_Compiler();

            foreach (Assembly assembly in GetReferencedAndInitialAssembly(Assembly.GetEntryAssembly()))
            {
                compiler.AddReferencedAssembly(assembly);
            }
        }

        /// <summary>
        /// Gather all assemblies referenced from current assembly.
        /// </summary>
        static public Assembly[] GetReferencedAndInitialAssembly(Assembly initialAssembly)
        {
            AssemblyName[] names = initialAssembly.GetReferencedAssemblies();
            Assembly[] assemblies = new Assembly[names.Length + 1];

            for (int i = 0; i < names.Length; i++)
            {
                assemblies[i] = Assembly.Load(names[i]);
            }

            assemblies[assemblies.Length - 1] = initialAssembly;

            return assemblies;
        }

        /// <summary>
        /// Load file, compile it and create/load the indicators into the CustomIndicatorsList.
        /// </summary>
        public void LoadCompileSourceFile(string filePath, out string errorMessages)
        {
            string errorLoadSourceFile;
            string source = LoadSourceFile(filePath, out errorLoadSourceFile);

            if (string.IsNullOrEmpty(source))
            {   // Source file loading failed.
                errorMessages = errorLoadSourceFile;
                return;
            }

            Dictionary<string, int> dictCompilationErrors;
            Assembly assembly = compiler.CompileSource(source, out dictCompilationErrors);

            if (assembly == null)
            {   // Assembly compilation failed.
                StringBuilder sbCompilationError = new StringBuilder();
                sbCompilationError.AppendLine("ERROR: Indicator compilation failed in file [" + Path.GetFileName(filePath) + "]");

                foreach (string error in dictCompilationErrors.Keys)
                {
                    sbCompilationError.AppendLine('\t' + error);
                }

                errorMessages = sbCompilationError.ToString();
                return;
            }

            string errorGetIndicator;
            Indicator newIndicator = GetIndicatorInstanceFromAssembly(assembly, out errorGetIndicator);

            if (newIndicator == null)
            {   // Getting an indicator instance failed.
                errorMessages = errorGetIndicator;
                return;
            }

            // Check for a repeated indicator name among the custom indicators
            foreach(Indicator indicator in listCustomIndicators)
                if (indicator.IndicatorName == newIndicator.IndicatorName)
                {   
                    errorMessages = "The name '" + newIndicator.IndicatorName + "' found out in [" + Path.GetFileName(filePath) + "] is already in use.";
                    return;
                }

            // Check for a repeated indicator name among the original indicators
            foreach (string indicatorName in Indicator_Store.OriginalIndicatorNames)
                if (indicatorName == newIndicator.IndicatorName)
                {
                    errorMessages = "The name '" + indicatorName + "' found out in [" + Path.GetFileName(filePath) + "] is already in use.";
                    return;
                }

            // Test the new custom indicator
            string errorTestIndicator;
            if (!Indicator_Tester.CustomIndicatorFastTest(newIndicator, out errorTestIndicator))
            {   // Testing the indicator failed.
                errorMessages = errorTestIndicator;
                return;
            }

            // Adds the custom indicator to the list
            listCustomIndicators.Add(newIndicator);

            errorMessages = string.Empty;
            return;
        }

        /// <summary>
        /// Reads the source code from file contents.
        /// </summary>
        public string LoadSourceFile(string pathToIndicator, out string errorLoadSourceFile)
        {
            string result = string.Empty;

            if (!File.Exists(pathToIndicator))
            {
                errorLoadSourceFile = "ERROR The source file does not exist: " + Path.GetFileName(pathToIndicator);
                return result;
            }

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(pathToIndicator))
                {
                    result = sr.ReadToEnd();
                    result = result.Replace("Forex_Strategy_Builder", "Forex_Strategy_Trader");
                    sr.Close();
                }
            }
            catch
            {
                errorLoadSourceFile = "ERROR Cannot read the file: " + Path.GetFileName(pathToIndicator);
                return result;
            }

            errorLoadSourceFile = string.Empty;
            return result;
        }

        /// <summary>
        /// Creates an indicator instance from the assembly given.
        /// </summary>
        static Indicator GetIndicatorInstanceFromAssembly(Assembly assembly, out string errorMessage)
        {
            Type[] assemblyTypes = assembly.GetTypes();
            foreach (Type typeAssembly in assemblyTypes)
            {
                if(typeAssembly.IsSubclassOf(typeof(Indicator)))
                {
                    ConstructorInfo[] aConstructorInfo = typeAssembly.GetConstructors();

                    // Looking for an appropriate constructor.
                    foreach (ConstructorInfo constructorInfo in aConstructorInfo)
                    {
                        ParameterInfo[] parameterInfo = constructorInfo.GetParameters();
                        if (constructorInfo.IsConstructor &&
                            constructorInfo.IsPublic      &&
                            parameterInfo.Length == 1     &&
                            parameterInfo[0].ParameterType == typeof(SlotTypes))
                        {
                            try
                            {
                                errorMessage = string.Empty;
                                return (Indicator)constructorInfo.Invoke(new object[] { SlotTypes.NotDefined });
                            }
                            catch (Exception exc)
                            {
                                errorMessage = "ERROR: [" + typeAssembly.Name + "] " + exc.Message;
                                if(!string.IsNullOrEmpty(exc.InnerException.Message))
                                    errorMessage += Environment.NewLine + "\t" + exc.InnerException.Message;
                                return null;
                            }

                        }
                    }

                    errorMessage = "ERROR: Cannot find an appropriate constructor for " + typeAssembly.Name + ".";
                    return null; 
                }
            }

            errorMessage = "ERROR: Cannot create an instance of an indicator from " + assembly.ToString() + ".";
            return null; 
        }
    }
}
