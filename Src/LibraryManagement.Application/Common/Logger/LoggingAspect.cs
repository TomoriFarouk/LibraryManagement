using NLog;
using PostSharp.Aspects;
using PostSharp.Serialization;

[PSerializable]
public class LogExecutionAttribute : OnMethodBoundaryAspect
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public override void OnEntry(MethodExecutionArgs args)
    {
        var methodName = args.Method.Name;
        var arguments = string.Join(", ", args.Arguments);

        // Log the method entry and request data
        Logger.Info($"Entering method {methodName} with arguments: {arguments}");
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        var methodName = args.Method.Name;
        var returnValue = args.ReturnValue;

        // Log the method exit and response data
        Logger.Info($"Exiting method {methodName} with return value: {returnValue}");
    }

    public override void OnException(MethodExecutionArgs args)
    {
        var methodName = args.Method.Name;
        var exception = args.Exception;

        // Log the exception
        Logger.Error(exception, $"Exception in method {methodName}");
    }
}
