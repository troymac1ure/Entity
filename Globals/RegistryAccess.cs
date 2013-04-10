using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Globals
{
    public class RegistryAccess
    {
        public bool isOpen { get { return _isOpen;} }
        private bool _isOpen = false;

        private RegistryKey the_Reg;

        public class RegPaths
        {
            public static string Halo2 = @"Software\Xbox\Halo2\";
            public static string Halo2Paths = Halo2 + @"Paths\";
            public static string Halo2RecentFiles = Halo2 + @"Recent Files\";
            public static string Halo2CustomPluigins = Halo2 + @"CustomPlugins\";
        }

        public class RegNames
        {
            public static string BitmapsFile = "Bitmaps";
            public static string BitmapsPath = "BitmapExtracts";
            public static string CleanMapsPath = "CleanMaps";
            public static string ExtractsPath = "Extracts";
            public static string MainMenuFile = "MainMenu";
            public static string MapsPath = "Maps";
            public static string FontsPath = "Fonts";
            public static string PluginsPath = "Plugins";
            public static string SharedFile = "Shared";
            public static string SinglePlayerSharedFile = "SPShared";
        }

        /// <summary>
        /// Opens access to reading the registry.
        /// Call the CloseReg() function when complete
        /// </summary>
        /// <param name="mainKey">Windows.Win32.Registry (Halo 2 uses CurrentUser)</param>
        /// <param name="subKey">The path to the Label</param>
        public RegistryAccess(RegistryKey mainKey, string subKey)
        {
            the_Reg = mainKey.OpenSubKey(subKey);
            if (the_Reg != null)
                _isOpen = true;
        }

        /// <summary>
        /// Closes the Registry read access.
        /// </summary>
        public void CloseReg()
        {
            if (_isOpen)
            try
            {
                the_Reg.Close();
                _isOpen = false;
            }
            catch {}
        }

        /// <summary>
        /// Sets the current mainkey & path
        /// </summary>
        /// <param name="mainKey">Windows.Win32.Registry (Halo 2 uses CurrentUser)</param>
        /// <param name="subKey">The path to the Label</param>
        public void setKey(RegistryKey mainKey, string subKey)
        {
            the_Reg = mainKey.OpenSubKey(subKey);
            if (the_Reg != null) _isOpen = true;
            else _isOpen = false;
        }


        /// <summary>
        /// Returns a value from the Label in the current path
        /// use setKey() to set the current mainKey & path
        /// </summary>
        /// <param name="valueName">The label name to return (Halo 2 uses RegistryAccess.RegNames)</param>
        /// <returns>The value of the Label</returns>
        public string getValue(string valueName)
        {
            if (!_isOpen) return null;
            object the_Obj;

            the_Obj = the_Reg.GetValue(valueName);
            if (the_Obj != null)
                return the_Obj.ToString();
            return null;
        }

        /// <summary>
        /// Sets a registry value
        /// </summary>
        /// <param name="mainKey">A Windows.Win32.Registry value (Halo2 uses CurrentUser)</param>
        /// <param name="subKey">The path to the value (Halo 2 uses RegistryAccess.RegPaths)</param>
        /// <param name="valueName">The Label of the value (Halo 2 uses RegistryAccess.RegNames)</param>
        /// <param name="value">The new value</param>
        /// <returns></returns>
        public static bool setValue(RegistryKey mainKey, string subKey, string valueName, object value)
        {
            RegistryKey the_Reg;
            try
            {
                the_Reg = mainKey.CreateSubKey(@subKey);
                the_Reg.SetValue(valueName, value);
                the_Reg.Close();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to remove a value from the registry
        /// </summary>
        /// <param name="mainKey">Windows.Win32.Registry (Halo 2 uses CurrentUser)</param>
        /// <param name="subKey">The path to the Label</param>
        /// <param name="valueName">The Value Label name to remove</param>
        /// <returns></returns>
        public static bool removeValue(RegistryKey mainKey, string subKey, string valueName)
        {
            RegistryKey the_Reg;
            try
            {
                the_Reg = mainKey.CreateSubKey(@subKey);
                the_Reg.DeleteValue(valueName);
                the_Reg.Close();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a file to the Halo 2 recently used list
        /// </summary>
        /// <param name="pos">The position in which to add the fileName (0-9)</param>
        /// <param name="fileName">The location and name of the file</param>
        public static void AddRecentFile(int pos, string fileName)
        {
            int found = -1;
            int count = 0;
            string[] files = new string[10];
            // Load recently used files list
            RegistryAccess ra = new RegistryAccess(Registry.CurrentUser, RegPaths.Halo2RecentFiles);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = ra.getValue(i.ToString());
                if (files[i] != null) count++;
                if (files[i] == fileName) found = i;
            }
            ra.CloseReg();

            if (found == -1) found = count;
            if (found < pos)
            {
                int z = found;
                found = pos;
                pos = z;
            }

            for (int i = found; i > pos; i--)
                setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2RecentFiles, i.ToString(), files[i-1]);
            setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2RecentFiles, pos.ToString(), fileName);
        }

        public string[] getKeys()
        {
            if (!_isOpen) return null;

            return the_Reg.GetSubKeyNames();
        }

        public string[] getNames()
        {
            if (!_isOpen) return null;

            return the_Reg.GetValueNames();
        }

        public object[] getValues()
        {
            if (!_isOpen) return null;
            string[] temp = the_Reg.GetValueNames();
            object[] tempValue = new object[temp.Length];
            for (int i = 0; i < temp.Length; i++ )
                tempValue[i] = the_Reg.GetValue(temp[i]);
            return tempValue;
        }

        public static bool removeKey(RegistryKey mainKey, string subKey)
        {
            try
            {
                mainKey.DeleteSubKeyTree(subKey);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Renames a subkey of the passed in registry key since 
        /// the Framework totally forgot to include such a handy feature.
        /// </summary>
        /// <param name="regKey">The RegistryKey that contains the subkey 
        /// you want to rename (must be writeable)</param>
        /// <param name="subKeyName">The name of the subkey that you want to rename
        /// </param>
        /// <param name="newSubKeyName">The new name of the RegistryKey</param>
        /// <returns>True if succeeds</returns>
        public static bool renameSubKey(RegistryKey parentKey, string subKeyName, string newSubKeyName)
        {
            copyKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
            return true;
        }

        /// <summary>
        /// Copy a registry key.  The parentKey must be writeable.
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="keyNameToCopy"></param>
        /// <param name="newKeyName"></param>
        /// <returns></returns>
        public static bool copyKey(RegistryKey parentKey, string keyNameToCopy, string newKeyName)
        {
            //Create new key
            RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName);

            //Open the sourceKey we are copying from
            RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy);

            recurseCopyKey(sourceKey, destinationKey);

            destinationKey.Close();
            sourceKey.Close();
            return true;
        }

        private static void recurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
                RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
                recurseCopyKey(sourceSubKey, destSubKey);
            }
        }
    }
}
