// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine;
// using UnityEngine.Timeline;
// using System.Linq;

// public class GameAgent : MonoBehaviour
// {
//     private Dictionary<Task, EventLauncher> eventLaunchers;

//     private void Awake()
//     {
//         eventLaunchers = GetComponentInChildren<TaskMarker>()
//             .ToDictionary(marker => marker.task,
//                 marker => marker.gameObject.GetComponent<EventLauncher>());
//     }
//     public static void PerformTheTask(GameAgent gameAgent, Task task, int parametr)
//     {
//         gameAgent.PerformTheTask(task, parametr);
//     }

//     public void PerformTheTask(Task task, int parametr)
//     {
//         eventLaunchers[task].Perform(parametr);
//     }
// }
