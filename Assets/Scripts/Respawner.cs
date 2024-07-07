using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] int minNumToRespawn;
    [SerializeField] int maxNumToRespawn;
    ITarget[] listTargets;
    private void Awake()
    {
        listTargets = GetComponentsInChildren<ITarget>();
    }

    private void OnEnable()
    {
        Clock.OnTimeChange += TimeChanged;
    }
    private void OnDisable()
    {
        Clock.OnTimeChange -= TimeChanged;
    }

    private void TimeChanged()
    {
        if (Clock.IsDay)
            return;

        var _SpawnNum = Random.Range(minNumToRespawn,maxNumToRespawn);

        if (IsEverythingEnebled(out var _DisabledNum))
            return;

        else if (_DisabledNum < _SpawnNum)
            _SpawnNum = _DisabledNum;

        Debug.Log($"name: {gameObject.name} "+_SpawnNum, transform);

        for (int i = 0; i < _SpawnNum; i++)
        {
            var rand = Random.Range(0, listTargets.Length);
            if (Respawn(rand) == false)
            {
                // if _DisabaledNum-- <= 0 then: _DisabaledNum = 0  else: _DisabaledNum--
                i = i - 1 <= 0 ? 0 : i--;
            }
        }
    }

    private bool IsEverythingEnebled(out int _DisabaledNum)
    {
        _DisabaledNum = 0;
        foreach (var _Item in listTargets)
        {
            if (_Item.MeshRenderer.enabled == false)
                _DisabaledNum++;
        }

        if (_DisabaledNum > 0)
            return false;

        return true;
    }

    bool Respawn(int _Index)
    {
        var _Item = listTargets[_Index];

        if (_Item.MeshRenderer.enabled)
        {
            return false;
        }
        Debug.Log(_Item.MeshRenderer.enabled, _Item.MyGameObject);
        _Item.MeshRenderer.enabled = true;
        return true;
    }
}
