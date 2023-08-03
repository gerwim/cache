namespace GerwimFeiken.Cache.Models;

public abstract class Result
{
    protected Result(Status operationStatus)
    {
        OperationStatus = operationStatus;
    }
    
    public Status OperationStatus { get; set; }
}

public class ReadResult<T> : Result
{
    public ReadResult(Status operationStatus, ReadReason? reason, T? value) : base(operationStatus)
    {
        Reason = reason;
        Value = value;
    }
     
    public static ReadResult<T> Ok( T value, ReadReason? reason = null) => new(Status.Ok, reason, value);
    public static ReadResult<T> Fail(T value, ReadReason? reason = null) => new(Status.Fail, reason, value);
    
    public T? Value { get; set; }
    
    public ReadReason? Reason { get; set; }
}

public class WriteResult : Result
{
    public WriteResult(Status operationStatus, WriteReason? reason = null) : base(operationStatus)
    {
        Reason = reason;
    }
    
    public static WriteResult Ok(WriteReason? reason = null) => new(Status.Ok, reason);
    public static WriteResult Fail(WriteReason? reason = null) => new(Status.Fail, reason);

    public WriteReason? Reason { get; set; }
}