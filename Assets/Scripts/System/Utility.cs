using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public enum GameTimeIndex
    {
        Noraml,

        Max
    }

    public static readonly Vector2 VECTOR2_ZERO = Vector2.zero;
    public static readonly Vector3 VECTOR3_ZERO = Vector3.zero;
    private static float[] _gameTimeArray = new float[(int)GameTimeIndex.Max];
    private static float[] _gameSpeedArray = new float[(int)GameTimeIndex.Max] { 1.0f };


    public enum LayerName
    {
        MAP3D01,

        CHARACTER3D01,

        Max,
    }

    private static string[] _layerNameArray = new string[(int)LayerName.Max]
    {
        "MAP3D01",
        "CHARACTER3D01",
    };


    public const float PI = 3.1415926535f;

    public static float Sin( float ang )
    {
        return Mathf.Sin(ang * PI / 180.0f);
    }

    public static float Cos(float ang)
    {
        return Mathf.Cos(ang * PI / 180.0f);
    }

    public static float GetAngle(float dirX, float dirZ)
    {
        float rad = Mathf.Atan2(dirX, dirZ);
        float degree = rad * Mathf.Rad2Deg;

        if (degree < 0)
        {
            degree += 360;
        }

        return degree;
    }

    public static float GetAngle(Vector3 dir)
    {
        return GetAngle( dir.x, dir.z );
    }

    public static Vector3 GetAngleVec(float ang)
    {
        return new Vector3(Sin(ang), 0, Cos(ang));
    }

    public static string GetLayerName(LayerName layerName)
    {
        return _layerNameArray[(int)layerName];
    }

    public static void Initialize( this Transform tr )
    {
        tr.localPosition = Vector3.zero;
        tr.localRotation = Quaternion.identity;
        tr.localScale = Vector3.one;
    }

    public static List<T> GetComponentAll<T>(this GameObject obj, ref List<T> allChildren ) where T : Component
    {
        GetChildren(obj, ref allChildren);
        return allChildren;
    }

    //子要素を取得してリストに追加
    private static void GetChildren<T>(GameObject obj, ref List<T> allChildren) where T : Component
    {
        T[] children = obj.GetComponentsInChildren<T>();
        //子要素がいなければ終了
        if (children.Length == 0)
        {
            return;
        }
        foreach (T ob in children)
        {
            if( ob.gameObject == obj )
            {
                continue;
            }
            allChildren.Add(ob);
            GetChildren(ob.gameObject, ref allChildren);
        }
    }

    /// <summary>
    /// 自分自身を含むすべての子オブジェクトのレイヤーを設定します
    /// </summary>
    public static void SetLayerRecursively(
        this GameObject self,
        LayerName layer
    )
    {
        SetLayerRecursively(self, LayerMask.NameToLayer(GetLayerName(layer)));
    }

    /// <summary>
    /// 自分自身を含むすべての子オブジェクトのレイヤーを設定します
    /// </summary>
    public static void SetLayerRecursively(
        this GameObject self,
        int layer
    )
    {
        self.layer = layer;

        foreach (Transform n in self.transform)
        {
            SetLayerRecursively(n.gameObject, layer);
        }
    }

    public static GameObject FindDeep(
        this GameObject self,
        string name,
        bool includeInactive = false)
    {
        var children = self.GetComponentsInChildren<Transform>(includeInactive);
        foreach (var transform in children)
        {
            if (transform.name == name)
            {
                return transform.gameObject;
            }
        }
        return null;
    }


    public static float GetGameTime( int timeIndex = 0 )
    {
        return _gameTimeArray[timeIndex];
    }

    public static void Update()
    {
        float time = Time.deltaTime;
        for (int i = 0; i < (int)GameTimeIndex.Max; i++)
        {
            _gameTimeArray[i] = _gameSpeedArray[i] * time;
        }
    }
}
