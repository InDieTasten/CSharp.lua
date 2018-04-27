
namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// A DiagnosticInfo object has information about a diagnostic, but without any attached location information.
    /// </summary>
    /// <remarks>
    /// More specialized diagnostics with additional information (e.g., ambiguity errors) can derive from this class to
    /// provide access to additional information about the error, such as what symbols were involved in the ambiguity.
    /// </remarks>
    // [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal class DiagnosticInfo // [Lua] : IFormattable, IObjectWritable
    {
        private readonly CommonMessageProvider _messageProvider;
        private readonly int _errorCode;
        private readonly DiagnosticSeverity _defaultSeverity;
        private readonly DiagnosticSeverity _effectiveSeverity;
        private readonly object[] _arguments;

        // Only the compiler creates instances.
        internal DiagnosticInfo(CommonMessageProvider messageProvider, int errorCode)
        {
            _messageProvider = messageProvider;
            _errorCode = errorCode;
            _defaultSeverity = messageProvider.GetSeverity(errorCode);
            _effectiveSeverity = _defaultSeverity;
        }


        // Only the compiler creates instances.
        internal DiagnosticInfo(CommonMessageProvider messageProvider, int errorCode, params object[] arguments)
            : this(messageProvider, errorCode)
        {
            // AssertMessageSerializable(arguments);

            _arguments = arguments;
        }
    }
}
