namespace Todo;

internal class MayWork
{
    public static void SomeMethod()
    {
        var hello = CoolGuy.Create<IHello>(new Func<string>(() => "hello world!"));
        Console.WriteLine(hello.SayHello());
    }
}

public static partial class CoolGuy
{
    public static partial T Create<T>(Func<string> param)
        where T : class;

    public static T Create<T>(object obj)
        => obj is null ? default : default;
}

public interface IHello
{
    string SayHello();
}