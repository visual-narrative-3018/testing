using UnityEngine;
using System;
using System.Collections;

public class SwitchPlaces : VRTK.VRTK_InteractableObject
{
    public int speed = 10;
    public int totalSwaps = 13;
    public GameObject[] originalArray = new GameObject[6];
    public GameObject obj;
    private readonly Vector3[] startingPositions = new Vector3[6];
    private GameObject[] orderedArray;
    private int total;
    private int chosenIndex = -1;
    private Boolean doneMoving = true;
    private Color color = Color.white;

    void Start()
    {
        orderedArray = (GameObject[]) originalArray.Clone();
        total = orderedArray.Length;
        UpdateStartingPositions();
        color.a = 0.66f;
        StartCoroutine(RandomizeOrder());
    }


    public override void StartUsing(VRTK.VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(currentUsingObject);
        if (doneMoving)
        {
            if (Array.IndexOf(orderedArray, obj) > -1)
            {
                int newIndex = Array.IndexOf(orderedArray, obj);
                if (chosenIndex != -1)
                {
                    orderedArray[chosenIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                }
                if (newIndex != chosenIndex)
                {
                    orderedArray[newIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
                    chosenIndex = newIndex;
                }
                else
                {
                    chosenIndex = -1;
                }
            }
        }
    }

    public override void StopUsing(VRTK.VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);
    }


    void UpdateStartingPositions()
    {
        for (int i = 0; i < total; i++)
        {
            startingPositions[i] = orderedArray[i].transform.position;
        }
    }

    IEnumerator RandomizeOrder()
    {
        for (int i = 0; i < totalSwaps; i++)
        {
            while (!doneMoving) {
                yield return null;
            }
            StartCoroutine(SwapPositions(UnityEngine.Random.Range(0, total), UnityEngine.Random.Range(0, total), true));
        }
    }

    IEnumerator SwapPositions(int a, int b, Boolean isStarting = false)
    {
        //To prevent the swap before another one is finished
        doneMoving = false;
        while (Vector3.Distance(orderedArray[a].transform.position, startingPositions[b]) > 0.01f ||
               Vector3.Distance(orderedArray[b].transform.position, startingPositions[a]) > 0.01f)
        {
            orderedArray[a].transform.position = Vector3.MoveTowards(orderedArray[a].transform.position, startingPositions[b], speed * Time.deltaTime);
            orderedArray[b].transform.position = Vector3.MoveTowards(orderedArray[b].transform.position, startingPositions[a], speed * Time.deltaTime);
            yield return null;
        }

        orderedArray[a].transform.position = startingPositions[b];
        orderedArray[b].transform.position = startingPositions[a];

        GameObject temp = orderedArray[a];
        orderedArray[a] = orderedArray[b];
        orderedArray[b] = temp;

        UpdateStartingPositions();
        doneMoving = isStarting || CheckComplete();
    }

    Boolean CheckComplete()
    {
        for (int i = 0; i < total; i++)
        {
            if (!originalArray[i].name.Equals(orderedArray[i].name))
            {
                return true;
            }
        }
        orderedArray[chosenIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        chosenIndex = -1;
        return false;
    }

    protected override void Update()
    {
        base.Update();
        //OnClick

        if (doneMoving && chosenIndex >= 0)
        {
            if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick) && chosenIndex > 0)
            {
                StartCoroutine(SwapPositions(chosenIndex--, chosenIndex));
            }

            if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick) && chosenIndex < total - 1)
            {
                StartCoroutine(SwapPositions(chosenIndex++, chosenIndex));
            }
        }
    }
}
