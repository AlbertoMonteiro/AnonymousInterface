using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace AnonymousInterface;
[Generator]
public class AnonymousInterfaceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is MySyntaxReceiver syntaxReceiver)
        {
            if (syntaxReceiver.DoIt)
            {
                var generatedCode = $@"using System;
namespace Todo
{{
    public static partial class CoolGuy
    {{
        public static partial T Create<T>(Func<string> param)
            where T : class
        {{
            if (typeof(T) == typeof({syntaxReceiver.InterfaceName}))
                return new GeneratedOla() as T;
            throw new System.Exception(""Some issue"");
        }}
        public class GeneratedOla : {syntaxReceiver.InterfaceName}
        {{
            public string SayHello() => {syntaxReceiver.Body.ToString()};
        }}
    }}
}}";
                context.AddSource("CoolGuy.generated.cs", SourceText.From(generatedCode, encoding: Encoding.UTF8));
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(MySyntaxReceiver.Creator);
}

public class MySyntaxReceiver : ISyntaxReceiver
{
    public static SyntaxReceiverCreator Creator => () => new MySyntaxReceiver();

    public bool DoIt { get; private set; }
    public ExpressionSyntax? Body { get; private set; }
    public string InterfaceName { get; private set; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is InvocationExpressionSyntax
            {
                ArgumentList:
                {
                    Arguments: { Count: 1 } arguments
                },
                Expression: MemberAccessExpressionSyntax
                {
                    Name: GenericNameSyntax
                    {
                        Identifier: { ValueText: "Create" },
                        TypeArgumentList:
                        {
                            Arguments: { Count: 1 } typeArgs,
                        },
                    }
                }
            } invocationExpression)
        {
            DoIt = true;
            Body = arguments.First().DescendantNodes().OfType<ParenthesizedLambdaExpressionSyntax>().First().ExpressionBody;
            InterfaceName = typeArgs.OfType<IdentifierNameSyntax>().First().Identifier.ValueText;
        }
    }
}
