using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Player _player;
    [SerializeField] private TMP_Text _below;
    [SerializeField] private TMP_Text _state;
    [SerializeField] private TMP_Text _velocity;

    [Header("Death")]
    [SerializeField] private float _deathShowUIDuration = 1.0f;
    [SerializeField] private float _deathHideUIDuration = 1.0f;
    [SerializeField] private GameObject _deathUI;

    private void Awake()
    {
        if(!instance) instance = this;
        _deathUI.SetActive(false);
    }

    private void FixedUpdate()
    {
        _below.SetText("isGrounded : " + _player.isGrounded());
        _velocity.SetText("Velocity : " + _player._velocity.ToString());
        //_wallSliding.SetText("Wall Sliding : " + _player._wallSliding);
        _state.SetText("State: " + _player._stateMachine.name());

        if (GameManager.instance.Notifications.death) { 
             StartCoroutine(ShowDeathUI(_deathUI, _deathShowUIDuration));
             StartCoroutine(HideDeathUI(_deathUI, _deathHideUIDuration));
        }

        DontDestroyOnLoad(gameObject);
    }

    IEnumerator ShowDeathUI(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }
    IEnumerator HideDeathUI(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
