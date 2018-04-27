using System;
using System.Collections.Generic;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// A diagnostic (such as a compiler error or a warning), along with the location where it occurred.
    /// </summary>
    // [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal class DiagnosticWithInfo : Diagnostic
    {
        private readonly DiagnosticInfo _info;
        private readonly Location _location;
        private readonly bool _isSuppressed;

        internal DiagnosticWithInfo(DiagnosticInfo info, Location location, bool isSuppressed = false)
        {
            //Debug.Assert(info != null);
            //Debug.Assert(location != null);
            _info = info;
            _location = location;
            _isSuppressed = isSuppressed;
        }

        public override DiagnosticDescriptor Descriptor => throw new NotImplementedException();

        public override string Id => throw new NotImplementedException();

        public override DiagnosticSeverity Severity => throw new NotImplementedException();

        public override int WarningLevel => throw new NotImplementedException();

        public override bool IsSuppressed => throw new NotImplementedException();

        public override Location Location => throw new NotImplementedException();

        public override IReadOnlyList<Location> AdditionalLocations => throw new NotImplementedException();

        public override string GetMessage(IFormatProvider formatProvider = null)
        {
            throw new NotImplementedException();
        }
    }
}
