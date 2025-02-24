using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameAgent : MonoBehaviour
{
    private Dictionary<Task, EventLauncher> eventLaunchers;
    public static void PerformTheTask(GameAgent gameAgent, Task<int> task, int parametr)
    {
        gameAgent.PerformTheTask(task, parametr);
    }

    public void PerformTheTask(Task<int> task, int parametr)
    {
        
    }
}
