namespace FileStorage
{
    public enum FileStorageResult
    {
        None = 0,
        Success = 1,
        
        FileAlreadyExists   = 10,
        FileNotFound        = 11,
        FileIsEmpty         = 12,
        WriteOperationError = 13
    }
}