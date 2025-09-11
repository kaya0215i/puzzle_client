using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class Functions {
    /// <summary>
    /// 目的の値に最も近い値を取得(float用)
    /// </summary>
    public static float Nearest(this IEnumerable<float> source, float targetValue) {
        if (source.Count() == 0) {
            Debug.LogError($"値が入っていないので、最も近い値を取得出来ません");
            return targetValue;
        }

        //目的の値との差の絶対値が最小の値を計算
        var min = source.Min(value => Mathf.Abs(value - targetValue));

        //絶対値が最小の値だった物を最も近い値として返す
        return source.First(value => Mathf.Approximately(Mathf.Abs(value - targetValue), min));
    }
}
