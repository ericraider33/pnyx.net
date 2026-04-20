namespace pnyx.net.fluent;

public interface INumberedInputOutput
{
    /// <summary>
    /// When no input (file or stream) is specified, this returns the first argument passed via command line.
    /// If no CMD arguments are provided, method returns null.
    /// </summary>
    /// <returns></returns>
    string? getImpliedInputFileName();
    
    /// <summary>
    /// When no output (file or stream) is specified, this returns the next argument passed via command line.
    /// If no CMD arguments are provided or all of arguments have been used, method returns null.
    /// </summary>
    /// <returns></returns>
    string? getImpliedOutputFileName();
    
    /// <summary>
    /// Returns CMD argument for given number, where 1 is the first/minimum value. If user hasn't provided enough
    /// arguments, method throws an InvalidArgumentException.
    /// </summary>
    /// <param name="argNumber"></param>
    /// <returns></returns>
    string getFileName(int argNumber);            // 1 is min value
    
    /// <summary>
    /// Return true if all user-provided arguments have been consumed / used.
    /// </summary>
    bool verifyAllUsed();
}