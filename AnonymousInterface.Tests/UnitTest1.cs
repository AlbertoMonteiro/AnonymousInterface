using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnonymousInterface.Tests;
public class UnitTest
{
    [Fact]
    public async Task Test1Async()
    {
        var sourceCode = @"using System;

namespace Todo
{
    public static partial class CoolGuy
    {
        public static partial T Create<T>(Func<string> param)
            where T : class;
    }
    public interface IOla
    {
        string SayHello();
    }
    public class Ola
    {
        public void M()
        {
            IOla ola = CoolGuy.Create<IOla>(new Func<string>(() => ""this is the hello""));
            ola.SayHello();
        }
    }
}";
        var fileName = @"AnonymousInterface\AnonymousInterface.AnonymousInterfaceGenerator\CoolGuy.generated.cs";
        var expectedSourceGenerated = @"using System;
namespace Todo
{
    public static partial class CoolGuy
    {
        public static partial T Create<T>(Func<string> param)
            where T : class
        {
            if (typeof(T) == typeof(IOla))
                return new GeneratedOla() as T;
            throw new System.Exception(""Some issue"");
        }
        public class GeneratedOla : IOla
        {
            public string SayHello() => ""this is the hello"";
        }
    }
}";

        await new CSharpSourceGeneratorTest<AnonymousInterfaceGenerator, XUnitVerifier>
        {
            TestState =
            {
                Sources ={sourceCode},
                GeneratedSources ={(fileName, SourceText.From(expectedSourceGenerated, Encoding.UTF8))}
            }
        }.RunAsync();
    }
}