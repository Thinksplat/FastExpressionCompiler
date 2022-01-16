using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if LIGHT_EXPRESSION
using static FastExpressionCompiler.LightExpression.Expression;
namespace FastExpressionCompiler.LightExpression.IssueTests
#else
using static System.Linq.Expressions.Expression;
namespace FastExpressionCompiler.IssueTests
#endif
{
    [TestFixture]
    public class OutParameterIssue : ITest
    {
        class TestPOD {
            public string stringvalue;
            public int intvalue;
        }

        public int Run()
        {
            Test_outparameter();
            return 1;
        }
        static private void TestStringOutMethod(string input, out string output) => output = input;
        static private void TestIntOutMethod(int input, out int output) => output = input;

        [Test]
        public void Test_outparameter()
        {
            var teststringoutmethodinfo = this.GetType().GetMethod("TestStringOutMethod", bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)!;
            var testintoutmethodinfo = this.GetType().GetMethod("TestIntOutMethod", bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)!;

            var pod = new TestPOD();

            var program = Expression.Block(
                Expression.Call(null, teststringoutmethodinfo,
                    Expression.Constant("hello world"), Expression.Field(Expression.Constant(pod), pod.GetType().GetField("stringvalue")!)),
                Expression.Call(null, testintoutmethodinfo,
                    Expression.Constant(4), Expression.Field(Expression.Constant(pod), pod.GetType().GetField("intvalue")!))
            );

            // Make a lambda and compile it
            var lambda = Expression.Lambda<Action>(program);

            // Compile this with fast and run
            var compiledfast = lambda.CompileFast();
                        compiledfast.PrintIL("fast");

            compiledfast();

        }
    }
}
