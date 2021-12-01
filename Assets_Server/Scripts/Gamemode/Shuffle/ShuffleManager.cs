using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

[System.Serializable]
public class Cup
{
    public Transform transform;
    public int position;
}

public class ShuffleManager : MonoBehaviour {

    public List<Cup> Cups;
    public List<Transform> Positions;
    public List<Transform> Waypoints;
    public float time = 1f;

    public Transform Ball;

    // Use this for initialization
    void Start () {
        DOTween.Init();
        StartCoroutine(Shuffle());
    }
	
	// Update is called once per frame
	void Update () {

    }

    IEnumerator Shuffle()
    {
        yield return new WaitForSeconds(1.5f);
        Cups[1].transform.DOMoveY(1, 0.5f).SetEase(Ease.OutCubic);
        Cups[1].transform.DORotate(new Vector3(30,0,0), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic); ;
        yield return new WaitForSeconds(2f);
        Cups[1].transform.DOMoveY(0.3f, 0.5f).SetEase(Ease.OutCubic); ;
        Cups[1].transform.DORotate(new Vector3(-30, 0, 0), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic); ;
        yield return new WaitForSeconds(1f);
        Ball.parent = Cups[1].transform;
        while (time > 0.15)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            int Start = Random.Range(0, Cups.Count);
            List<int> Valid = Enumerable.Range(0, Cups.Count).ToList();
            Valid.RemoveAt(Start);
            //Debug.Log(Valid.Count);
            int end = Valid[Random.Range(Valid[0], Valid.Count - 1)];

            //int OtherCup = end;

            int endPos = Cups[end].position;
            int startPos = Cups[Start].position;

            Switch(Start, Cups[end].position, 0);
            Switch(end, Cups[Start].position, 1);
            yield return new WaitForSeconds(time);
            Cups[Start].position = endPos;
            Cups[end].position = startPos;
            time = time - (time / 10);
        }
        Ball.parent = null;
        yield return new WaitForSeconds(0.5f);
        Cups[1].transform.DOMoveY(1, 0.5f).SetEase(Ease.OutCubic); ;
    }

    public void Switch(int Cup, int Target, int Waypoint)
    {
        //Debug.Log("Cup:"+Cup+", Target:"+Target+", Waypoint:"+Waypoint);
        Vector3[] Path = new[] { Cups[Cup].transform.position, Waypoints[Waypoint].position, Positions[Target].position };
        Cups[Cup].transform.DOPath(Path, time, PathType.CatmullRom).SetEase(Ease.OutCubic); ;
    }
}
