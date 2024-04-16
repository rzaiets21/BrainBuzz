using System;
using System.Collections.Generic;
using System.Linq;
using FileStorage;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;

public class StorageController : MonoBehaviour
{
    private const string BucketName = "gs://brainbuzztest.appspot.com";
    private const long MaxAllowedSize = 4 * 1024 * 1024;
    
    private readonly Dictionary<string, Sprite> _cache = new();
    
    private FileStorageController FileStorageController { get; set; }

    private void Awake()
    {
        FileStorageController = new FileStorageController();
    }

    public void Get(string url, Action<Sprite> onComplete, Action<float> onProgress = null)
    {
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            throw new Exception("Invalid url");
        
        var firebaseStorage = FirebaseStorage.GetInstance(BucketName);

        if (!TryGetFileReference(uri, firebaseStorage, out var fileReference))
            throw new Exception("File not found!");
        
        var filePath = GetCacheFilePath(firebaseStorage, fileReference.Name);
        
        if (FileStorageController.Load(filePath, out var cachedBytes, out var reason))
        {
            if (!TryParseBytesResult(cachedBytes, out var result))
            {
                var exception = new Exception("Get file failed : Unknown target type");
                Debug.LogError(exception.Message);
                onComplete?.Invoke(result);
                return;
            }
                
            _cache[url] = result;

            onComplete?.Invoke(result);
            return;
        }
        
        GetFileBytesFromFirebase(fileReference, bytes =>
            {
                if (!TryParseBytesResult(bytes, out var result))
                {
                    var exception = new Exception("Get file failed : Unknown target type");
                    Debug.LogError(exception.Message);
                    onComplete?.Invoke(result);
                    return;
                }

                FileStorageController.Save(filePath, bytes, out _);
                _cache[url] = result;
                onComplete?.Invoke(result);
            }, onProgress: onProgress);
    }

    public bool FileExist(string url)
    {
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            throw new Exception("Invalid url");
        
        var firebaseStorage = FirebaseStorage.GetInstance(BucketName);

        if (!TryGetFileReference(uri, firebaseStorage, out var fileReference))
            throw new Exception("File not found!");
        
        var filePath = GetCacheFilePath(firebaseStorage, fileReference.Name);
        return FileStorageController.Exist(filePath);
    }
    
    public void Get(List<string> urls, Action<Dictionary<string, Sprite>> onComplete, Action<float> onProgress = null)
    {
        if (urls.Count == 0)
        {
            onComplete?.Invoke(new Dictionary<string, Sprite>());
            return;
        }

        var distinctUrls = urls.Distinct().ToArray();
        var count = distinctUrls.Length;

        var list = distinctUrls.Select(url => new DownloadImageData(url)).ToList();
        var dict = new Dictionary<string, Sprite>(count);

        void OnResult(int index, Sprite result)
        {
            var url = list[index].ImageUrl;
            dict.TryAdd(url, result);

            if (dict.Count == count)
                onComplete?.Invoke(dict);
        }

        void OnProgress(int index, float progress)
        {
            list[index].DownloadProgress = progress;
            onProgress?.Invoke(list.Sum(x => x.DownloadProgress) / count);
        }

        for (int i = 0; i < distinctUrls.Length; i++)
        {
            var index = i;
            var url = distinctUrls[i];
            
            Get(url, (sprite) => OnResult(index, sprite), onProgress: (p) =>
            {
                OnProgress(index, p);
            });
        }
    }
    
    private void GetFileBytesFromFirebase(StorageReference reference, Action<byte[]> onComplete, Action<Exception> onError = null, Action<float> onProgress = null)
    {
        var downloadProgressHandler = new Progress<DownloadState>();
        downloadProgressHandler.ProgressChanged += (sender, state) =>
        {
            var progress = (float) state.BytesTransferred / state.TotalByteCount;
            onProgress?.Invoke(progress);
        };
        var tasks = reference.GetBytesAsync(MaxAllowedSize, downloadProgressHandler).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                onComplete.Invoke(task.Result);
            }
        });
    }
    
    private bool TryParseBytesResult(byte[] bytes, out Sprite result)
    {
        try
        {
            result = SpriteFromBytes(bytes);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }

        result = null;
        return false;
    }
    
    private Texture2D TextureFromBytes(byte[] bytes)
    {
        var texture = new Texture2D(1, 1);
        texture.LoadImage(bytes, false);
        return texture;
    }
    
    private Sprite SpriteFromBytes(byte[] bytes)
    {
        var texture = TextureFromBytes(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
    }
    
    private string GetCacheFilePath(FirebaseStorage storage, string fileName)
    {
        var folderName = GetCacheStorageFolderName(storage);
        return folderName == null ? fileName : $"{folderName}/{fileName}";
    }
    
    private string GetCacheStorageFolderName(FirebaseStorage storage)
    {
        return storage?.Url().Replace("gs://", string.Empty);
    }
    
    private bool TryGetFileReference(Uri uri, FirebaseStorage storage, out StorageReference reference)
    {
        reference = null;

        try
        {
            reference = storage.GetReferenceFromUrl(uri.AbsoluteUri);
        }
        catch
        {
            return false;
        }
        
        return true;
    }
}

[Serializable]
public class DownloadImageData
{
    public string ImageUrl;
    public float DownloadProgress;
    public Sprite Image;

    public DownloadImageData(string url)
    {
        ImageUrl = url;
    }
}
