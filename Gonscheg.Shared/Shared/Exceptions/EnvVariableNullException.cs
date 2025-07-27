namespace Gonscheg.Shared.Exceptions;

public class EnvVariableNullException(string envVariableName)
    : NullReferenceException($"Environment variable {envVariableName} is not initialized");