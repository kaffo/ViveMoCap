using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoCap : MonoBehaviour
{
    public float frameDelay = 0.005f;
    public int maxSize = 100000;
    public GameObject[] toTrack;

    private bool active;
    private int frameNo;
    private float timePassed;
    private TrackedInfo[,] trackedInfo;

    class TrackedInfo
    {
        public Vector3 position;
        public Quaternion rotation;

        public TrackedInfo(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    // Use this for initialization
    void Start()
    {
        active = true;
        frameNo = 0;
        timePassed = 0f;
        trackedInfo = new TrackedInfo[maxSize, toTrack.Length];
    }

    IEnumerator writeOutput()
    {
        Debug.Log("Writing to File...");
        List<string> toWrite = new List<string>();
        toWrite.Add("[Header]");
        toWrite.Add("FileType htr");
        toWrite.Add("DataType HTRS");
        toWrite.Add("FileVersion 1");
        toWrite.Add("NumSegments 18");
        toWrite.Add("NumFrames 2");
        toWrite.Add("DataFrameRate 60");
        toWrite.Add("EulerRotationOrder ZYX");
        toWrite.Add("CalibrationUnits mm");
        toWrite.Add("RotationUnits Degrees");
        toWrite.Add("GlobalAxisofGravity Y");
        toWrite.Add("BoneLengthAxis Y");
        toWrite.Add("ScaleFactor 1.000000");

        toWrite.Add("[SegmentNames&Hierarchy]");
        toWrite.Add("#CHILD PARENT");
        toWrite.Add("Head Hips");
        toWrite.Add("LeftWrist Head");
        toWrite.Add("RightWrist Head");
        toWrite.Add("Hips GLOBAL");
        toWrite.Add("LeftAnkle Hips");
        toWrite.Add("RightAnkle Hips");

        //TODO replace these so they are actually right
        toWrite.Add("[BasePosition]");
        toWrite.Add("#SegmentName Tx, Ty, Tz, Rx, Ry, Rz, BoneLength");
        toWrite.Add("Head	0.3214	0.5230	-0.0553	-0.0519	0.4612	0.0536	0.8842	0.5");
        toWrite.Add("LeftWrist	0.3114	-1.0531	0.0593	-0.1691	0.8834	0.0533	0.4337	0.63");
        toWrite.Add("RightWrist	0.4900	-1.0625	-0.1395	0.0093	0.9919	0.1104	0.0624	0.63");
        toWrite.Add("Hips	0	0	0	0	0	0	0");
        toWrite.Add("LeftAnkle	-0.1538	0.3440	0.5332	-0.5511	-0.2455	-0.3584	0.7124	0.85");
        toWrite.Add("RightAnkle	0.6767	0.3721	-0.6897	0.0020	0.6600	0.6736	0.3326	0.85");

        toWrite.Add("[Head]");
        for (int i = 0; i < toTrack.Length; i++)
        {
            toWrite.Add("[" + toTrack[i].name + "]");
            for (int j = 0; j < frameNo; j++)
            {
                TrackedInfo t = trackedInfo[j, i];                
                //TODO get a proper per bone scaling 
                toWrite.Add(j + " " + t.position.x + "\t" + t.position.y + "\t" + t.position.z + "\t" + t.rotation.eulerAngles.x + "\t" + t.rotation.eulerAngles.y + "\t" + t.rotation.eulerAngles.z + " " + "1");
            }
        }
        toWrite.Add("[EndOfFile]");
        System.IO.File.WriteAllLines("G:\\mocapoutput.txt", toWrite.ToArray());
        Debug.Log("Write Complete!");
        yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed += Time.fixedDeltaTime;
        if (active && timePassed >= frameDelay && frameNo < maxSize)
        {
            timePassed = 0f;
            for (int i = 0; i < toTrack.Length; i++)
            {
                trackedInfo[frameNo, i] = new TrackedInfo(toTrack[i].transform.localPosition, toTrack[i].transform.localRotation);
            }
            frameNo++;
        }
        if (Input.GetKeyDown(KeyCode.Space) && active)
        {
            Debug.Log("Stopping MoCap...");
            active = false;
            StartCoroutine("writeOutput");
            Debug.Log("MoCap Stopped!");
        }
    }
}
