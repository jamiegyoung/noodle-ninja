using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator animator;

    public enum Levels
    {
        Menu,
        Intro,
        Game
    }

    public void LoadLevel(Levels levels)
    {
        StartCoroutine(AnimationLoad(levels));
    }

    private IEnumerator AnimationLoad(Levels level)
    {
        animator.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene((int)level);
    }
}
