using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updater : MonoBehaviour
{
    /// <summary>
    /// —Dæ‡
    /// </summary>
    protected int _updatePriority = 0;
    public int UpdatePriority { get { return _updatePriority; } }

    public virtual void PreAlterUpdate() { }

    public virtual void AlterUpdate() { }
}
