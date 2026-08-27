using System.Threading.Tasks;

namespace Engine;

/// <summary>
/// PROBLEMATICO SE CASO TIVER MULTIPLAS CENAS, POIS AS TASKS ESTAO SENDO APLICADAS EM UM LISTA ESTATICA SEM DISTINÇÃO DE CENAS
/// </summary>

public static class TaskRunner
{
    private static readonly List<Action> _tasksToExecute = new();
    private static readonly List<Action> _tasksActive = new();

    public static Task Yield()
    {
        var tcs = new TaskCompletionSource<bool>();
        lock (_tasksToExecute){
            _tasksToExecute.Add(() => tcs.SetResult(true));
        }
        return tcs.Task;
    }

    public static async Task WaitForSeconds(float seconds)
    {
        float time = Time.gameTime + seconds;
        while(time > Time.gameTime) await Yield();
    }

    public static void Update()
    {
        _tasksActive.Clear();

        lock (_tasksToExecute)
        {
            if (_tasksToExecute.Count == 0) return;
            _tasksActive.AddRange(_tasksToExecute);
            _tasksToExecute.Clear();
        }

        foreach(var task in _tasksActive){
            task.Invoke();
        }
    }
}



