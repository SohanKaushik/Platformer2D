using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [SerializeField] GameObject _respawn;
    public struct GameNotifications
    {
        public bool death;
    }

    private GameNotifications _notifications;
    public GameNotifications Notifications => _notifications;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NotifyDeath()
    {
        _notifications.death = true;
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public IEnumerator Respawn(float delay, GameObject target) {
        yield return new WaitForSeconds(delay);
        target.transform.position = _respawn.transform.position;
        _notifications.death = false;
    }

    public void ClearNotifications()
    {
        _notifications = new GameNotifications();
    }

}
