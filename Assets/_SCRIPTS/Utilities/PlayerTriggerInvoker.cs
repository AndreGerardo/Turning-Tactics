using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerInvoker : MonoBehaviour
{

    public UnityEvent<Character_Player> OnTriggerEnterEvent;
    public UnityEvent<Character_Player> OnTriggerStayEvent;
    public UnityEvent<Character_Player> OnTriggerExitEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character_Player>(out Character_Player player))
        {
            OnTriggerEnterEvent?.Invoke(player);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character_Player>(out Character_Player player))
        {
            OnTriggerStayEvent?.Invoke(player);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character_Player>(out Character_Player player))
        {
            OnTriggerExitEvent?.Invoke(player);
        }
    }

}
