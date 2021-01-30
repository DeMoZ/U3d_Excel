using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task) tcs.Task).GetAwaiter();
    }
    
    // var getRequest = UnityWebRequest.Get("http://www.google.com");
    // await getRequest.SendWebRequest();
    // var result = getRequest.downloadHandler.text;
}