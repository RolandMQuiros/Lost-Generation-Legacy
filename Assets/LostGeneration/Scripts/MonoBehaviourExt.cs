using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExt {

    #region WaitForMethods
    public static Coroutine WaitForCoroutines(this MonoBehaviour component, IEnumerable<IEnumerator> routines) {
        return component.StartCoroutine(component.RunParallelCoroutines(routines));
    }

    private static IEnumerator RunParallelCoroutines(this MonoBehaviour component, IEnumerable<IEnumerator> routines) {
        HashSet<Coroutine> running = new HashSet<Coroutine>();

        foreach (IEnumerator routine in routines) {
            Coroutine run = component.StartCoroutine(component.RunParallelRoutine(routine, running));
        }

        while (running.Count > 0) {
            yield return null;
        }
    }

    private static IEnumerator RunParallelRoutine(this MonoBehaviour component, IEnumerator routine, HashSet<Coroutine> running) {
        Coroutine run = component.StartCoroutine(routine);
        running.Add(run);
        yield return run;
        running.Remove(run);
    }
    #endregion WaitForMethods

}
