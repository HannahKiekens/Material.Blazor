﻿namespace Material.Blazor
{
    /// <summary>
    /// An interface implemented by <see cref="MBDialog"/> to allow child components to
    /// register themselves for Material Theme js instantiation.
    /// </summary>
    public interface IMBDialog
    {
        /// <summary>
        /// The child component should implement <see cref="DialogChildComponent"/> and call this when running <code>OnInitialized()</code>.
        /// </summary>
        /// <param name="child">The child components that implements <see cref="DialogChildComponent"/></param>
        void RegisterLayoutAction(DialogChildComponent child);


        /// <summary>
        /// True once the dialog has instantiated components for the first time.
        /// </summary>
        bool DialogHasInstantiated { get; }
    }
}
