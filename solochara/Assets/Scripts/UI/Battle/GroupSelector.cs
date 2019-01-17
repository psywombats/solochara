using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupSelector : MonoBehaviour {

    public Doll dollPrefab;
    private List<Doll> dolls;

    public IEnumerator SelectAnyOneRoutine(Result<BattleUnit> result, Intent intent) {
        yield break;
    }

    public IEnumerator SelectAnyExceptRoutine(Result<BattleUnit> result, Intent intent) {
        yield break;
    }

    public IEnumerator SelectAllRoutine(Result<List<BattleUnit>> result, Intent intent) {
        yield break;
    }

    public IEnumerator SelectAllExceptRoutine(Result<List<BattleUnit>> result, Intent intent) {
        yield break;
    }

    public IEnumerator SelectSpecificallyRoutine(Intent intent) {
        yield break;
    }
}
