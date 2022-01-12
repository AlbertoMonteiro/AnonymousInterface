using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnonymousInterface.Tests;
public class UnitTest : CSharpSourceGeneratorTest<AnonymousInterfaceGenerator, XUnitVerifier>
{
    [Fact]
    public async Task Test1Async()
    {
        TestState.Sources.Add(@"using System;

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
}");
        TestState.GeneratedSources.Add((@"AnonymousInterface\AnonymousInterface.AnonymousInterfaceGenerator\CoolGuy.generated.cs", SourceText.From(@"using System;
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
}", Encoding.UTF8)));
        await RunAsync();
    }
}