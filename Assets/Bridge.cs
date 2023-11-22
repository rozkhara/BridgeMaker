using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public List<Vector2> westSitePosList;
    public List<Vector2> eastSitePosList;

    [SerializeField]
    private List<Vector2> westSitePosList_sorted;
    [SerializeField]
    private List<Vector2> eastSitePosList_sorted;

    [SerializeField]
    private List<Vector2Int> BridgeList;    //Contains westIndex, eastIndex in sorted list

    private List<List<Vector2Int>> StateList;
    [SerializeField]
    private List<float> Distance;    //Distance of state[idx];

    private bool isMade = false;
    private int minIndex = -1;
    private int maxIndex = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetAll();
        }
        if (Input.GetKeyDown(KeyCode.L))    //check BridgeList in inspector to see this state
        {
            BridgeList = GetLeast();
        }
        if (Input.GetKeyDown(KeyCode.G))    //check BridgeList in inspector to see this state
        {
            BridgeList = GetGreatest();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetState();
        }
    }

    private void ResetState()
    {
        if (isMade)
        {
            BridgeList.Clear();
            StateList.ForEach(l => l.Clear());
            StateList.Clear();
            Distance.Clear();
            westSitePosList_sorted.Clear();
            eastSitePosList_sorted.Clear();
            isMade = false;
            minIndex = -1;
            maxIndex = -1;
        }
    }

    public List<List<Vector2Int>> GetAll()
    {
        MakeBridge();
        return StateList;
    }
    public List<Vector2Int> GetLeast()
    {
        MakeBridge();
        if (minIndex == -1)
        {
            minIndex = Distance.IndexOf(Distance.Min());
        }
        return StateList[minIndex];
    }
    public List<Vector2Int> GetGreatest()
    {
        MakeBridge();
        if (maxIndex == -1)
        {
            maxIndex = Distance.IndexOf(Distance.Max());
        }
        return StateList[maxIndex];
    }

    private void Sort()
    {
        westSitePosList_sorted = new List<Vector2>();
        westSitePosList_sorted = westSitePosList.OrderBy(v => v.y).ToList();
        eastSitePosList_sorted = new List<Vector2>();
        eastSitePosList_sorted = eastSitePosList.OrderBy(v => v.y).ToList();
    }

    private int CCW(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float s = p1.x * p2.y + p2.x * p3.y + p3.x * p1.y;
        s -= (p1.y * p2.x + p2.y * p3.x + p3.y * p1.x);
        if (s > 0) return 1;
        else if (s == 0) return 0;
        else return -1;
    }

    private bool CheckCross(Vector2 startPos, Vector2 endPos)
    {
        bool isIntersectFound = false;
        foreach (var bridge in BridgeList)
        {
            Vector2 b_start = westSitePosList_sorted[bridge.x];
            Vector2 b_end = eastSitePosList_sorted[bridge.y];
            if (endPos.y == b_end.y)
            {
                isIntersectFound = true;
                break;
            }
            int p1p2 = CCW(startPos, endPos, b_start) * CCW(startPos, endPos, b_end);
            int p3p4 = CCW(b_start, b_end, startPos) * CCW(b_start, b_end, endPos);
            isIntersectFound = p1p2 <= 0 && p3p4 <= 0;
            if (isIntersectFound) break;
        }
        return isIntersectFound;
    }

    private void MakeBridge()
    {
        if (isMade) return;
        Sort();
        BridgeList = new List<Vector2Int>();
        StateList = new List<List<Vector2Int>>();
        Distance = new List<float>();
        _makeBridge(0, 0, 0.0f);
        isMade = true;
    }

    private void _makeBridge(int a, int b, float dist)
    {
        if (a == westSitePosList_sorted.Count)
        {
            StateList.Add(BridgeList.ConvertAll(b => b));
            Distance.Add(dist);
            return;
        }
        for (int i = b; i < eastSitePosList_sorted.Count; ++i)
        {
            bool isPossible = !CheckCross(westSitePosList_sorted[a], eastSitePosList_sorted[i]);
            if (isPossible)
            {
                Vector2Int idx = new Vector2Int(a, i);
                BridgeList.Add(idx);
                _makeBridge(a + 1, 0, dist + Vector2.Distance(westSitePosList_sorted[a], eastSitePosList_sorted[i]));
                BridgeList.Remove(idx);
            }
        }
    }


}
