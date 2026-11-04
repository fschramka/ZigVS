namespace ZigVS.Common
{
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Threading;

#nullable enable
    /// <summary>
    /// Tracked JoinableTaskFactory for fire-and-forget work.
    /// ZigVSPackage derives from the synchronous MPF Package (no AsyncPackage.JoinableTaskFactory),
    /// and some callers are MEF parts that may run before the package loads. Work started through
    /// this factory is joined in ZigVSPackage.Dispose so it completes before VS shuts down (VSSDK007).
    /// </summary>
    internal static class ZigVSThreadHelper
    {
        private static readonly JoinableTaskCollection s_Collection =
            ThreadHelper.JoinableTaskContext.CreateCollection();

        public static JoinableTaskFactory JoinableTaskFactory { get; } =
            ThreadHelper.JoinableTaskContext.CreateFactory(s_Collection);

        /// <summary>Blocks until all work started via <see cref="JoinableTaskFactory"/> has finished.</summary>
        public static void JoinPendingWork()
        {
            ThreadHelper.JoinableTaskFactory.Run(() => s_Collection.JoinTillEmptyAsync());
        }
    }
}
