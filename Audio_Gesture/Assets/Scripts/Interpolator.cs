using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator {

    //Can at most increase the list by original size - 2, run multiple times to increase it by more.
    //Well, currently it can only increase the size by approximately 50%
	public static List<Vector3> interpolate(List<Vector3> listToInterpolate, int targetSize)
    {
        if(listToInterpolate.Count > targetSize)
        {
            return decreaseSizeInterpolate(listToInterpolate, targetSize);
        }

        List<Vector3> interpolateList = listToInterpolate;
        int sizeDifference = targetSize - listToInterpolate.Count;

        if (sizeDifference == 0)
        {
            return interpolateList;
        }

        List<int> indices = new List<int>();
        float originIndex = (float)listToInterpolate.Count / (float)sizeDifference;
        for (int i = 0; i < sizeDifference; i++)
        {
            indices.Add((int)Mathf.Round(originIndex * ((float)i + 1f)));
        }
        if(indices[0] == 0)
        {
            indices.RemoveAt(0);
            sizeDifference--;
            Debug.Log("Size reduced because index 0 was 0");
            targetSize--;
        }

        if (indices[indices.Count-1] >= listToInterpolate.Count-1)
        {
            indices.RemoveAt(indices.Count - 1);
            sizeDifference--;
            Debug.Log("Size reduced because index last was the last of listToInterpolate");
            targetSize--;
        }
        //This is not perfect, but it will do for now.
        for (int i = 0; i < sizeDifference; i++)
        {
            interpolateList[indices[i] + i + 1] = (interpolateList[indices[i] + i + 2] + interpolateList[indices[i] + i + 1]) / 2f;
            interpolateList[indices[i] + i + -1] = (interpolateList[indices[i] + i - 2] + interpolateList[indices[i] + i - 1]) / 2f;
            Vector3 newPoint = (interpolateList[indices[i] + i + 1] + interpolateList[indices[i] + i - 1]) / 2f;
            interpolateList.Insert(indices[i] + i, newPoint);
        }

        return interpolateList;
    }

    static List<Vector3> decreaseSizeInterpolate(List<Vector3> listToInterpolate, int targetSize)
    {

        List<Vector3> interpolateList = listToInterpolate;
        int sizeDifference = listToInterpolate.Count - targetSize;

        List<int> indices = new List<int>();
        float originIndex = (float)listToInterpolate.Count / (float)sizeDifference;
        for (int i = 0; i < sizeDifference; i++)
        {
            indices.Add((int)Mathf.Round(originIndex * ((float)i + 1f)));
        }

        if (indices[0] == 0)
        {
            indices.RemoveAt(0);
            sizeDifference--;
            Debug.Log("Size reduced because index 0 was 0");
        }
        if (indices[indices.Count - 1] >= listToInterpolate.Count - 1)
        {
            indices.RemoveAt(indices.Count - 1);
            sizeDifference--;
            Debug.Log("Size reduced because index last was the last of listToInterpolate");
        }

        //This is not perfect, but it will do for now.
        for (int i = 0; i < sizeDifference; i++)
        {
            interpolateList[indices[i] - i + 1] = (interpolateList[indices[i] - i + 2] + interpolateList[indices[i] - i + 1] + interpolateList[indices[i] - i]) / 3f;
            interpolateList[indices[i] - i + -1] = (interpolateList[indices[i] - i - 2] + interpolateList[indices[i] - i - 1] + interpolateList[indices[i] - i]) / 3f;
            interpolateList.RemoveAt(indices[i] - i);
        }

        return interpolateList;
    }
}
