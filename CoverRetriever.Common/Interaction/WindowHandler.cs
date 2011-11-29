// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowHandler.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  This class monitor all windows in application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Interaction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// This class monitor all windows in application.
    /// </summary>
    public static class WindowHandler
    {
        /// <summary>
        /// List of handled windows.
        /// </summary>
        private static readonly List<Window> HandledWindows = new List<Window>();

        /// <summary>
        /// Add window for observe. If Window already exists - skip operation.
        /// </summary>
        /// <param name="window">Window for track.</param>
        public static void SafeAddWindow(Window window)
        {
            if (!HandledWindows.Contains(window))
            {
                HandledWindows.Add(window);
            }
        }

        /// <summary>
        /// Remove window from observe. If Window already absent - skip operation.
        /// </summary>
        /// <param name="window">Window stop for track.</param>
        public static void SafeRemoveWindow(Window window)
        {
            if (HandledWindows.Contains(window))
            {
                HandledWindows.Remove(window);
            }
        }

        /// <summary>
        /// Close all window and clear observable windows.
        /// </summary>
        public static void CloseAllWindow()
        {
            while (HandledWindows.Any())
            {
                var window = HandledWindows.First();
                WindowExtensions.SetHideOnClose(window, false);
                window.Close();
            }
        }
    }
}