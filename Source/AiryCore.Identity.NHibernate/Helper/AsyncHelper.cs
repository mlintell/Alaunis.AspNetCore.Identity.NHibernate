namespace AiryCore.Helper
{
    using System;
    using System.Threading.Tasks;

    internal static class AsyncHelper
    {
        public static Task<TResult> FromCanceled<TResult>()
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetCanceled();
            return tcs.Task;
        }

        public static Task<TResult> FromException<TResult>(Exception exc)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exc);
            return tcs.Task;
        }
    }
}