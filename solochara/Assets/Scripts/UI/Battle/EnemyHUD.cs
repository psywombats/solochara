using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class EnemyHUD : MonoBehaviour {

    public SliderBar hpBar;
    public Text text;
    public float fadeTime = 0.1f;

    public void Start() {
        GetComponent<CanvasGroup>().alpha = 0.0f;
        gameObject.SetActive(false);
    }

    public void Populate(BattleUnit unit) {
        text.text = unit.unit.unitName;
        hpBar.Populate(unit.Get(StatTag.MHP), unit.Get(StatTag.HP));
    }

    public IEnumerator enableRoutine(BattleUnit unit) {
        Populate(unit);
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            yield return CoUtils.RunTween(GetComponent<CanvasGroup>().DOFade(1.0f, fadeTime));
        }
    }

    public IEnumerator disableRoutine() {
        if (gameObject.activeSelf) {
            yield return CoUtils.RunTween(GetComponent<CanvasGroup>().DOFade(0.0f, fadeTime));
            gameObject.SetActive(false);
        }
    }
}
