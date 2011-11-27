using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;

using NUnit.Framework;

namespace CoverRetriever.Test
{
    /// <summary>
    /// Contains common utility methods which should use in Unit Tests.
    /// </summary>
    public static class ResourceUtils
    {
        private const string TestInputDir = "Input";

        /// Returns name of current test method by using information which stored in <see cref="StackTrace"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetTestName()
        {
            string currentTestMethodName = null;

            var stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                MethodBase method = stackTrace.GetFrame(i).GetMethod();
                if (IsMethodContainsAttribute(method, typeof(TestAttribute)) ||
                    IsMethodContainsAttribute(method, typeof(TestCaseAttribute)))
                {
                    currentTestMethodName = method.Name;
                    break;
                }
            }

            if (currentTestMethodName != null)
            {
                return currentTestMethodName;
            }

            throw new InvalidOperationException("Unable to determine current Unit test name");
        }

        public static string GetTestClassName()
        {
            string currentTestClassName = null;

            var stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                MethodBase method = stackTrace.GetFrame(i).GetMethod();
                if (IsMethodContainsAttribute(method, typeof(TestAttribute)) ||
                    IsMethodContainsAttribute(method, typeof(TestCaseAttribute)))
                {
                    currentTestClassName = method.DeclaringType.Name;
                    break;
                }
            }

            if (currentTestClassName != null)
            {
                return currentTestClassName;
            }

            throw new InvalidOperationException("Unable to determine current Unit test name");
        }

        /// <summary>
        /// Returns namespace of current test method by using information which stored in <see cref="StackTrace"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetTestNamespace()
        {
            string currentTestClassName = null;

            var stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                MethodBase method = stackTrace.GetFrame(i).GetMethod();
                if (IsMethodContainsAttribute(method, typeof(TestAttribute)) ||
                    IsMethodContainsAttribute(method, typeof(TestCaseAttribute)))
                {
                    currentTestClassName = method.DeclaringType.Namespace;
                    break;
                }
            }

            if (currentTestClassName != null)
            {
                return currentTestClassName;
            }

            throw new InvalidOperationException("Unable to determine current Unit test name");
        }

        /// <summary>
        /// Get resource stram by file name for current test 
        /// <remarks>
        ///  Stream must close manually
        /// </remarks>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream GetTestInputStream(string fileName)
        {
            string testNamespace = GetTestNamespace();
            string currentAssemblyFullName = Assembly.GetCallingAssembly().FullName;
            string currentAssemblyName = currentAssemblyFullName.Substring(0, currentAssemblyFullName.IndexOf(","));

            string inputDirectory = testNamespace.Replace(currentAssemblyName, TestInputDir);
            string resourceUri = String.Format("{0};component/{1}/{2}", currentAssemblyName, inputDirectory, fileName);
            StreamResourceInfo fileStream = Application.GetResourceStream(new Uri(resourceUri, UriKind.Relative));

            if (fileStream != null)
            {
                return fileStream.Stream;
            }
            throw new InvalidOperationException(String.Format("Resource {0} not found", resourceUri));
        }

        private static bool IsMethodContainsAttribute(MethodBase method, Type attribute)
        {
            return method.GetCustomAttributes(attribute, false).Length > 0;
        }
    }
}