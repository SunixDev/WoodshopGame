using UnityEngine;
using System.Collections;

public interface IToolManager
{
    CutLine GetNearestLine(Vector3 fromPosition);
}
