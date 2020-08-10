using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEffectAnimation : MonoBehaviour
{
    public Text score;

    Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        animator
            .runtimeAnimatorController
            .animationClips
            .ToList()
            .ForEach(
            clip => clip.wrapMode = WrapMode.Once
            );


        FindObjectOfType<WallBuilder>().WallPassed += OnWallPassed;
        AddScoreTriggeredEventListeners();
        TextRefresh();

        var booster = FindObjectOfType<Booster>();
        booster.BoostEnabled += OnBoostChanged;
        booster.BoostDisabled += OnBoostChanged;
    }

    private void Update()
    {
        //var lvlMultiplier = GameManager.LEVEL > 1? $"×{GameManager.LEVEL} " : "";

        //var multiplier = lvlMultiplier + (GameManager.BOOST? $"x{GameManager.SCORE_BONUS}" : "");
        //var multiplier = GameManager.BOOST ? $"x{GameManager.SCORE_BONUS}" : "";
        //score.text = $"{GameManager.SCORE}\n{multiplier}";
    }

    void OnBoostChanged(object sender, EventArgs args)
    {
        TextRefresh();
    }

    //async void OnScoreTriggered()
    //{
    //    animator.Play(
    //        $"Base Layer.{(GameManager.BOOST? "BoostEffect" : "DefaultEffect")}", 0
    //        );

    //    while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
    //    {
    //        await Task.Yield();
    //    }
    //    TextRefresh();
    //    //anim.Stop();
    //}

    void OnScoreTriggered()
    {
        animator.Play(
            $"Base Layer.{(GameManager.BOOST ? "BoostEffect" : "DefaultEffect")}", 0
            );

        TextRefresh();
        //anim.Stop();
    }

    void OnWallPassed(object sender, EventArgs args)
    {
        AddScoreTriggeredEventListeners();
        TextRefresh();
    }

    void AddScoreTriggeredEventListeners()
    {
        FindObjectsOfType<ScoreTrigger>().ToList()
            .ForEach(trigger =>
            trigger.ScoreTriggered.AddListener(OnScoreTriggered)
            );
    }

    void TextRefresh()
    {
        var multiplier = GameManager.BOOST ? $"×{GameManager.SCORE_BONUS}" : "";
        score.text = $"{GameManager.SCORE}\n{multiplier}";
    }
}
