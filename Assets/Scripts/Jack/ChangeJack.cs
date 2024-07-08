using System.Collections.Generic;
using UnityEngine;

public class ChangeJack : MonoBehaviour
{
    List<Jack> jacks = new();
    int index;
    private void Awake()
    {
        jacks.AddRange(GetComponentsInChildren<Jack>());
    }

    private void Start()
    {
       for (int i = 0; i < jacks.Count; i++)
        {
            Jack jack = jacks[i];
            jack.number.text = (i+1).ToString();
        }

        UpdateMainJack();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            index--;
            if (index < 0)
                index = jacks.Count - 1;

            UpdateMainJack();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            index++;
            if (index > jacks.Count - 1)
                index = 0;

            UpdateMainJack();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (jacks.Count - 1 == 0)
                return;

            jacks[index].gameObject.SetActive(false);
            jacks.RemoveAt(index);

            index = 0;
            UpdateMainJack();
        }
    }

    void UpdateMainJack()
    {
        for (int i = 0; i < jacks.Count; i++)
        {
            if (i != index)
            {
                jacks[i].jacksCam.enabled = false;
                jacks[i].jacksCam.gameObject.tag = "UwU";
            }
            else
            {
                jacks[i].jacksCam.enabled = true;
                jacks[i].jacksCam.gameObject.tag = "MainCamera";
            }
        }
    }
}
