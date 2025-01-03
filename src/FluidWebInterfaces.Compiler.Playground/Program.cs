using System.Net;
using FluidWebInterfaces.Compiler.Core.Compilation;

namespace FluidWebInterfaces.Compiler.Playground;

class Program
{
    static void Main(string[] args)
    {
        HttpListenerA();
    }

    static async void HttpListenerA()
    {
        using HttpListener listener = new();
        listener.Start();

        listener.Prefixes.Add("http://localhost:5000/index/");

        var context = await listener.GetContextAsync();
        Console.WriteLine(context.Request.Url!.AbsoluteUri);
    }
}