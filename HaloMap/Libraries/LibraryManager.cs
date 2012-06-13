namespace HaloMap.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using entity;

    /// <summary>
    /// The plugin manager.
    /// </summary>
    /// <remarks></remarks>
    public class LibraryManager
    {
        #region Constants and Fields

        /// <summary>
        /// The plugin.
        /// </summary>
        public LibraryManagerList Plugin = new LibraryManagerList();

        #endregion

        /// <summary>
        /// The plugin manager list.
        /// </summary>
        /// <remarks></remarks>
        public class LibraryManagerList : List<EntityPlugin>
        {
            #region Public Methods

            /// <summary>
            /// The add.
            /// </summary>
            /// <param name="pluginsPath">The plugins path.</param>
            /// <remarks></remarks>
            public void Add(string pluginsPath)
            {
                try
                {
                    foreach (FileInfo file in new DirectoryInfo(pluginsPath).GetFiles())
                    {
                        Assembly asm;
                        try
                        {
                            asm = Assembly.LoadFile(file.FullName);
                        }
                        catch
                        {
                            continue;
                        }

                        try
                        {
                            foreach (Type type in asm.GetTypes())
                            {
                                // foreach (Type interf in type.GetType())
                                if (type.BaseType == typeof(EntityPlugin))
                                {
                                    this.Add((EntityPlugin)Activator.CreateInstance(type));
                                }
                            }

                        }
                        catch //(System.Reflection.ReflectionTypeLoadException e)
                        {

                        }
                    }
                }
                catch
                {
                }
            }

            #endregion
        }
    }
}