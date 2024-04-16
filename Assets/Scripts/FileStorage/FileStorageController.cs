using System.IO;
using UnityEngine;

namespace FileStorage
{
    public class FileStorageController
    {
        public bool Save(string filePath, byte[] bytes, out FileStorageResult result)
        {
            var path = PrepareFilePath(filePath);
            if (File.Exists(path))
            {
                result = FileStorageResult.FileAlreadyExists;
                return false;
            }

            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch
            {
                result = FileStorageResult.WriteOperationError;
                return false;
            }

            result = FileStorageResult.Success;
            return true;
        }

        public bool Exist(string filePath)
        {
            var path = PrepareFilePath(filePath);
            return File.Exists(path);
        }
        
        public bool Load(string filePath, out byte[] bytes, out FileStorageResult result)
        {
            bytes = null;

            var path = PrepareFilePath(filePath);
            if (!File.Exists(path))
            {
                result = FileStorageResult.FileNotFound;
                return false;    
            }
            
            bytes = File.ReadAllBytes(path);
            if (bytes == null)
            {
                result = FileStorageResult.FileIsEmpty;
                return false;
            }

            result = FileStorageResult.Success;
            return true;
        }
    
        private string PrepareFilePath(string filePath)
        {
            // #if UNITY_ANDROID && !UNITY_EDITOR
            //
            // // Automatic => /storage/emulated/0/Android/data/[PACKAGE_NAME]/files: OK
            //
            // // Internal, Persistent, Absolute => /data/user/0/[PACKAGE_NAME]/files: OK
            // // Internal, Persistent, Default => /data/user/0/[PACKAGE_NAME]/files: OK
            // // Internal, Persistent, Canonical => /data/data/[PACKAGE_NAME]/files: OK
            //
            // // Internal, Cached, Absolute => /data/user/0/[PACKAGE_NAME]/cache: OK
            // // Internal, Cached, Default => /data/user/0/[PACKAGE_NAME]/cache: OK
            // // Internal, Cached, Canonical => /data/data/[PACKAGE_NAME]/cache: OK
            //
            // // External, Persistent, Absolute => /storage/emulated/0/Android/data/[PACKAGE_NAME]/files: OK
            // // External, Persistent, Default => /storage/emulated/0/Android/data/[PACKAGE_NAME]/files: OK
            // // External, Persistent, Canonical => /storage/emulated/0/Android/data/[PACKAGE_NAME]/files: OK
            //
            // // External, Cached, Absolute => /storage/emulated/0/Android/data/[PACKAGE_NAME]/cache: OK
            // // External, Cached, Default => /storage/emulated/0/Android/data/[PACKAGE_NAME]/cache: OK
            // // External, Cached, Canonical => /storage/emulated/0/Android/data/[PACKAGE_NAME]/cache: OK
            //
            // var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // var activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            //
            // var endMethod = "getCanonicalPath";
            //
            // return activity.Call<AndroidJavaObject>("getFilesDir").Call<string>(endMethod);
            //
            // #else
            //
            // // PathAttributes are applicable only to Android platform at the moment.
            // return Application.persistentDataPath;
            //
            // #endif
            
            var path = $"{Application.persistentDataPath}/Cache/{filePath}";
            var directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName) && directoryName != null)
                Directory.CreateDirectory(directoryName);
            return path;
        }
    }
}